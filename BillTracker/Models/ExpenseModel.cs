using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseModel
    {
        internal ExpenseModel(Expense expense)
        {
            Id = expense.Id;
            Name = expense.Name;
            Amount = expense.Amount;
            ExpenseTypeId = expense.ExpenseTypeId;
            AggregateId = expense.AggregateId;
        }

        public Guid Id { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public Guid? ExpenseTypeId { get; }

        public Guid AggregateId { get; }
    }
}
