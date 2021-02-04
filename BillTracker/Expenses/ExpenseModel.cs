using System;
using BillTracker.Entities;

namespace BillTracker.Expenses
{
    public class ExpenseModel
    {
        internal ExpenseModel(Expense expense)
        {
            Id = expense.Id;
            AddedById = expense.AddedById;
            AddedAt = expense.AddedAt;
            Amount = expense.Amount;
        }

        public Guid Id { get; }
        public Guid AddedById { get; }
        public DateTimeOffset AddedAt { get; }
        public decimal Amount { get; }
    }
}
