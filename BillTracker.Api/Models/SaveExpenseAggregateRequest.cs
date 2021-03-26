using System;

namespace BillTracker.Api.Models
{
    public class SaveExpenseAggregateRequest
    {
        public Guid? Id { get; set; }

        public DateTimeOffset? AddedDate { get; set; }

        public string Name { get; set; }

        public bool IsDraft { get; set; }
    }
}
