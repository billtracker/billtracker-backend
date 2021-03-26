using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseAggregateModel
    {
        internal ExpenseAggregateModel(ExpensesAggregate entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            UserId = entity.UserId;
        }

        public Guid Id { get; }

        public string Name { get; }

        public Guid UserId { get; }
    }
}
