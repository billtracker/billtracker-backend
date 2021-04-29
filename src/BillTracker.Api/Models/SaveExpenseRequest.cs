using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Api.Models
{
    public record SaveExpenseRequest
    {
        [Required]
        public Guid? AggregateId { get; init; }

        public Guid? ExpenseId { get; init; }

        [Required]
        public string Name { get; init; }

        public decimal Price { get; init; }

        [Range(1, int.MaxValue)]
        public int Amount { get; init; }

        public Guid? ExpenseTypeId { get; init; }
    }
}
