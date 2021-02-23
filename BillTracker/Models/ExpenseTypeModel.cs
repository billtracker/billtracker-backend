using System;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public class ExpenseTypeModel
    {
        internal ExpenseTypeModel(ExpenseType expenseType)
        {
            Id = expenseType.Id;
            UserId = expenseType.UserId;
            Name = expenseType.Name;
            IsBuiltIn = expenseType.IsBuiltIn;
        }

        public Guid Id { get; }

        public Guid? UserId { get; }

        public string Name { get; }

        public bool IsBuiltIn { get; }
    }
}
