using System;

namespace BillTracker.Api.Models
{
    public record SaveExpenseAggregateRequest
    {
        public Guid? Id { get; init; }

        public DateTimeOffset? AddedDate { get; init; }

        public string Name { get; init; }

        public bool IsDraft { get; init; }
    }
}
