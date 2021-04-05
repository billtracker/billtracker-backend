using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleHashing.Net;

namespace BillTracker.Identity
{
    public interface IIdentityService
    {
        Task<ResultOrError<AuthenticationResult>> Login(string emailAddressOrUserName, string password);

        Task<SuccessOrError> Register(string emailAddress, string password, string userName);

        Task<ResultOrError<AuthenticationResult>> RefreshToken(string refreshToken);

        Task<SuccessOrError> RevokeToken(string token);
    }

    internal class IdentityService : IIdentityService
    {
        private readonly BillTrackerContext _context;
        private readonly IdentityConfiguration _configuration;
        private readonly SetupNewUser _setupNewUser;

        public IdentityService(
            BillTrackerContext context,
            IdentityConfiguration configuration,
            SetupNewUser setupNewUser)
        {
            _context = context;
            _configuration = configuration;
            _setupNewUser = setupNewUser;
        }

        public async Task<ResultOrError<AuthenticationResult>> Login(string emailAddressOrUserName, string password)
        {
            var user = await _context.Users
                .Include(x => x.RefreshToken)
                .SingleOrDefaultAsync(x =>
                    x.EmailAddress.ToLower() == emailAddressOrUserName.ToLower() ||
                    x.UserName.ToLower() == emailAddressOrUserName.ToLower());

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

            await _setupNewUser.Handle(user.Id);

            return new AuthenticationResult
            {
                AccessToken = jwt.AccessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = jwt.ExpiresAt,
            };
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

            return new AuthenticationResult
            {
                AccessToken = newJwt.AccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newJwt.ExpiresAt,
            };
        }

        public async Task<SuccessOrError> Register(string emailAddress, string password, string userName)
        {
            var emailAddressTaken = await _context.Users
                .AnyAsync(x =>
                    x.EmailAddress.ToLower() == emailAddress.ToLower() ||
                    x.UserName.ToLower() == userName.ToLower());
            if (emailAddressTaken)
            {
                return IdentityErrors.EmailTaken;
            }

            var hashedPassword = new SimpleHash().Compute(password);
            var newUser = User.Create(emailAddress, hashedPassword, userName);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return SuccessOrError.FromSuccess();
        }

        public async Task<SuccessOrError> RevokeToken(string token)
        {
            var oldRefreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Token == token);
            if (oldRefreshToken == null)
            {
                return IdentityErrors.InvalidRefreshToken;
            }

            _context.Remove(oldRefreshToken);
            await _context.SaveChangesAsync();

            return SuccessOrError.FromSuccess();
        }

        private static RefreshToken GenerateRefreshToken(Guid userId)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return Entities.RefreshToken.Create(userId, Convert.ToBase64String(randomBytes));
        }

        private (string AccessToken, DateTimeOffset ExpiresAt) GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.Secret);
            var expiresIn = _configuration.AccessTokenValidity.HasValue
                ? DateTime.UtcNow.Add(_configuration.AccessTokenValidity.Value)
                : DateTime.UtcNow.AddMinutes(10);

            var accessToken = new JwtSecurityTokenHandler()
                .CreateEncodedJwt(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(IdentityClaims.UserId, user.Id.ToString()),
                        new Claim(IdentityClaims.EmailAddress, user.EmailAddress),
                        new Claim(IdentityClaims.UserName, user.UserName),
                    }),
                    Expires = expiresIn,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration.Issuer,
                });

            return (accessToken, expiresIn);
        }
    }
}
