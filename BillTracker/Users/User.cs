using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Users
{
    public class User
    {
        public static User Create(string firstName, string lastName, string password)
            => new User(id: Guid.NewGuid(),
                        firstName: firstName,
                        lastName: lastName,
                        password: password,
                        createdAt: DateTimeOffset.Now);

        public User(Guid id, string firstName, string lastName, string password, DateTimeOffset createdAt)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            CreatedAt = createdAt;
        }

        public Guid Id { get; private set; }

        [Required]
        public string FirstName { get; private set; }

        [Required]
        public string LastName { get; private set; }

        [Required]
        public string Password { get; private set; }

        [Required]
        public DateTimeOffset CreatedAt { get; private set; }
    }
}
