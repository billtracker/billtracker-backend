using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleHashing.Net;

namespace BillTracker.Identity
{
    public interface IIdentityService
    {
        Task<ResultOrError<AuthenticationResult>> Login(string emailAddress, string password);
        Task<SuccessOrError> Register(string emailAddress, string password, string firstName, string lastName);
        Task<ResultOrError<AuthenticationResult>> RefreshToken(string token);
        Task<SuccessOrError> RevokeToken(string token);
    }

    internal class IdentityService : IIdentityService
    {
        private readonly BillTrackerContext _context;
        private readonly IdentityConfiguration _configuration;

        public IdentityService(BillTrackerContext context, IdentityConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ResultOrError<AuthenticationResult>> Login(string emailAddress, string password)
        {
            var user = await _context.Users
                .Include(x => x.RefreshToken)
                .SingleOrDefaultAsync(x => x.EmailAddress.ToLower() == emailAddress.ToLower());

            if (user == null || !new SimpleHash().Verify(password, user.Password))
            {
                return IdentityErrors.InvalidUser;
            }

            if (user.RefreshToken != null)
            {
                _context.Remove(user.RefreshToken);
            }

            var jwt = GenerateJwtToken(user);

            var refreshToken = GenerateRefreshToken(user.Id);
            await _context.RefreshTokens.AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            return new AuthenticationResult(jwt.AccessToken, refreshToken.Token, jwt.ExpiresAt);
        }

        public async Task<ResultOrError<AuthenticationResult>> RefreshToken(string refreshToken)
        {
            var oldRefreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (oldRefreshToken == null)
            {
                return IdentityErrors.InvalidRefreshToken;
            }

            var newJwt = GenerateJwtToken(oldRefreshToken.User);

            var newRefreshToken = GenerateRefreshToken(oldRefreshToken.UserId);
            _context.Remove(oldRefreshToken);
            await _context.AddAsync(newRefreshToken);

            await _context.SaveChangesAsync();

            return new AuthenticationResult(newJwt.AccessToken, newRefreshToken.Token, newJwt.ExpiresAt);
        }

        public async Task<SuccessOrError> Register(string emailAddress, string password, string firstName, string lastName)
        {
            var emailAddressTaken = await _context.Users.AnyAsync(x => x.EmailAddress.ToLower() == emailAddress.ToLower());
            if (emailAddressTaken)
            {
                return IdentityErrors.EmailTaken;
            }

            var hashedPassword = new SimpleHash().Compute(password);
            var newUser = User.Create(emailAddress, hashedPassword, firstName, lastName);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return SuccessOrError.FromSuccess();
        }

        public async Task<SuccessOrError> RevokeToken(string refreshToken)
        {
            var oldRefreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (oldRefreshToken == null)
            {
                return IdentityErrors.InvalidRefreshToken;
            }

            _context.Remove(oldRefreshToken);
            await _context.SaveChangesAsync();

            return SuccessOrError.FromSuccess();
        }

        private (string AccessToken, DateTimeOffset ExpiresAt) GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.Secret);
            var expiresIn = DateTime.UtcNow.AddMinutes(10);

            var accessToken = new JwtSecurityTokenHandler()
                .CreateEncodedJwt(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(IdentityClaims.UserId, user.Id.ToString()),
                        new Claim(IdentityClaims.EmailAddress, user.EmailAddress),
                        new Claim(IdentityClaims.UserName, $"{user.FirstName} {user.LastName}"),
                    }),
                    Expires = expiresIn,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration.Issuer,
                });

            return (accessToken, expiresIn);
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return Entities.RefreshToken.Create(userId, Convert.ToBase64String(randomBytes));
        }
    }
}
