using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BillTracker.Shared;
using BillTracker.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleHashing.Net;

namespace BillTracker.Identity
{
    public interface IIdentityService
    {
        Task<SuccessOrError> Register(string emailAddress, string password, string firstName, string lastName);

        Task<ResultOrError<JwtTokenResult>> Login(string emailAddress, string password);
    }

    internal class IdentityService : IIdentityService
    {

        private readonly BillTrackerContext _context;

        public IdentityService(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<JwtTokenResult>> Login(string emailAddress, string password)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.EmailAddress.ToLower() == emailAddress.ToLower());

            if (user == null || !new SimpleHash().Verify(password, user.Password))
            {
                return IdentityErrors.InvalidUser;
            }

            return GenerateJwtToken(user);
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

        private static JwtTokenResult GenerateJwtToken(User user)
        {
            var validTo = DateTime.UtcNow.AddDays(7); // Finally must be reduced to 1 hour

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("--X BILL TRACKER DEV X--");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(IdentityClaims.UserId, user.Id.ToString()),
                    new Claim(IdentityClaims.EmailAddress, user.EmailAddress),
                    new Claim(IdentityClaims.UserName, $"{user.FirstName} {user.LastName}"),
                }),
                Expires = validTo,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "https://billtracker/identity",
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new JwtTokenResult(accessToken, validTo);
        }
    }
}
