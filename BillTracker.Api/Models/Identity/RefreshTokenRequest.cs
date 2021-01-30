using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models.Identity
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
