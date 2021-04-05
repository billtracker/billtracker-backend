using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; init; }

        [Required]
        [MinLength(5)]
        public string Password { get; init; }

        [Required]
        [MinLength(2)]
        public string UserName { get; init; }
    }
}
