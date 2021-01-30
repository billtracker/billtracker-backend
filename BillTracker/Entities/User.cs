using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    public class User
    {
        public static User Create(string emailAddress, string password, string firstName, string lastName)
            => new User
            {
                Id = Guid.NewGuid(),
                Password = password,
                EmailAddress = emailAddress,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTimeOffset.Now,
            };

        public Guid Id { get; private set; }

        [Required]
        public string EmailAddress { get; private set; }

        [Required]
        public string FirstName { get; private set; }

        [Required]
        public string LastName { get; private set; }

        [Required]
        public string Password { get; private set; }

        [Required]
        public DateTimeOffset CreatedAt { get; private set; }

        public RefreshToken RefreshToken { get; private set; }
    }
}
