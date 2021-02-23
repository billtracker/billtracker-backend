using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseModel
    {
        internal ExpenseModel(Expense expense)
        {
            Id = expense.Id;
            AddedById = expense.UserId;
            AddedAt = expense.AddedDate;
            Name = expense.Name;
            Amount = expense.Amount;

            ExpenseTypeId = expense.ExpenseTypeId;
            ExpenseTypeName = expense.ExpenseType.Name;
        }

        public Guid Id { get; }

        public Guid AddedById { get; }

        public DateTimeOffset AddedAt { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public Guid ExpenseTypeId { get; }

        public string ExpenseTypeName { get; }
    }
}
