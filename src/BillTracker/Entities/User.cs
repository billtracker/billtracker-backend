using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class User : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public string EmailAddress { get; private set; }

        [Required]
        public string UserName { get; private set; }

        [Required]
        public string Password { get; private set; }

        [Required]
        public DateTimeOffset CreatedAt { get; private set; }

        public RefreshToken RefreshToken { get; private set; }

        [Required]
        public bool WasSetup { get; private set; }

        public static User Create(string emailAddress, string password, string userName)
            => new User
            {
                Id = Guid.NewGuid(),
                Password = password,
                EmailAddress = emailAddress,
                UserName = userName,
                CreatedAt = DateTimeOffset.Now,
            };

        public void Setup()
        {
            WasSetup = true;
        }
    }
}
