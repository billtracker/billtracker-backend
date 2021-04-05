using System;

namespace BillTracker.Models
{
    public record ExpenseBillFileModel
    {
        public Guid Id { get; init; }

        public Guid AggregateId { get; init; }

        public string FileName { get; init; }

        public Uri FileUri { get; init; }
    }
}
