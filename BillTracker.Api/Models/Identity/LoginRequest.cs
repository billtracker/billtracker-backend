using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public class LoginRequest
    {
        [Required]
        public string EmailAddressOrUserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
