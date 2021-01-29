using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
