using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(5)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
