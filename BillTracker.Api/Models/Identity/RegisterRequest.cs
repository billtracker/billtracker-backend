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
        [MinLength(2)]
        public string UserName { get; set; }
    }

    public class RegisterResponse
    {
        public RegisterResponse(string emailAddress, string userName)
        {
            EmailAddress = emailAddress;
            UserName = userName;
        }

        public string EmailAddress { get; }

        public string UserName { get; }
    }
}
