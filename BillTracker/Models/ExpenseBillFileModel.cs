using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseBillFileModel
    {
        internal ExpenseBillFileModel(ExpenseBillFile entity)
        {
            Id = entity.Id;
            AggregateId = entity.AggregateId;
            FileName = entity.FileName;
            FileUri = entity.FileUri;
        }

        public Guid Id { get; }

        public Guid AggregateId { get; }

        public string FileName { get; }

        public Uri FileUri { get; }
    }
}
