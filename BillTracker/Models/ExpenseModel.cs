using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseModel
    {
        internal ExpenseModel(Expense expense)
        {
            Id = expense.Id;
            UserId = expense.Aggregate.UserId;
            AddedDate = expense.Aggregate.AddedDate;
            Name = expense.Name;
            Amount = expense.Amount;

            ExpenseTypeId = expense.ExpenseTypeId;
            ExpenseTypeName = expense.ExpenseType.Name;

            AggregateId = expense.Aggregate.Id;
            AggregateName = expense.Aggregate.Name;
        }

        public Guid Id { get; }

        public Guid UserId { get; }

        public DateTimeOffset AddedDate { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public Guid ExpenseTypeId { get; }

        public string ExpenseTypeName { get; }

        public Guid AggregateId { get; }

        public string AggregateName { get; }
    }
}
