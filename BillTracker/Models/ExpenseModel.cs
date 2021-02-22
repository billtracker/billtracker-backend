using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseModel
    {
        public ExpenseModel(Guid id, Guid addedById, DateTimeOffset addedAt, string name, decimal amount)
        {
            Id = id;
            AddedById = addedById;
            AddedAt = addedAt;
            Name = name;
            Amount = amount;
        }

        internal ExpenseModel(Expense expense)
        {
            Id = expense.Id;
            AddedById = expense.AddedById;
            AddedAt = expense.AddedAt;
            Name = expense.Name;
            Amount = expense.Amount;
        }

        public Guid Id { get; }

        public Guid AddedById { get; }

        public DateTimeOffset AddedAt { get; }

        public string Name { get; }

        public decimal Amount { get; }
    }
}
