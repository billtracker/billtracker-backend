using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public record CreateExpenseTypeRequest
    {
        [Required]
        public string Name { get; init; }
    }
}
