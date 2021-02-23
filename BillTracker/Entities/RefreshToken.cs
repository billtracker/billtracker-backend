using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class RefreshToken : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public Guid UserId { get; private set; }

        public User User { get; private set; }

        [Required]
        public string Token { get; private set; }

        [Required]
        public DateTimeOffset CreatedAt { get; private set; }

        [Required]
        public DateTimeOffset ValidTo { get; private set; }

        public static RefreshToken Create(Guid userId, string token) => new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTimeOffset.Now,
            ValidTo = DateTimeOffset.Now.AddDays(7),
        };
    }
}
