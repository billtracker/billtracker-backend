using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public class AddExpenseRequest
    {
        public Guid? AggregateId { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset? AddedDate { get; set; }

        public Guid? ExpenseTypeId { get; set; }
    }
}
