using System;

namespace BillTracker.Models
{
    public record ExpenseModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public decimal Amount { get; init; }

        public Guid? ExpenseTypeId { get; init; }

        public Guid AggregateId { get; init; }
    }
}
