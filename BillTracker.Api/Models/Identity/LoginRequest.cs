using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public class LoginRequest
    {
        [Required]
        public string EmailAddressOrUserName { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
