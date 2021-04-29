using System;

namespace BillTracker.Api.Models
{
    public record SaveExpenseAggregateRequest
    {
        public Guid? AggregateId { get; init; }

        public DateTimeOffset? AddedDate { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public bool IsDraft { get; init; }
    }
}
