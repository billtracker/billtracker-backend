using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public class RevokeTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
