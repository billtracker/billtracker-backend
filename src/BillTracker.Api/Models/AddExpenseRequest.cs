using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public record AddExpenseRequest
    {
        public Guid? AggregateId { get; init; }

        [Required]
        public string Name { get; init; }

        public decimal Amount { get; init; }

        public DateTimeOffset? AddedDate { get; init; }

        public Guid? ExpenseTypeId { get; init; }
    }
}
