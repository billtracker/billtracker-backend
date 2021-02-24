using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public class CreateExpenseTypeRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
