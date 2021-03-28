using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class ExpenseBillFile : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public Guid AggregateId { get; private set; }

        public ExpensesAggregate Aggregate { get; private set; }

        [Required]
        public string FileName { get; private set; }

        [Required]
        public Uri FileUri { get; private set; }

        public static ExpenseBillFile Create(
            Guid aggregateId,
            string fileName,
            Uri fileUri) => new ExpenseBillFile
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                FileName = fileName,
                FileUri = fileUri,
            };
    }
}
