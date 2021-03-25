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
        }

        public Guid Id { get; }

        public Guid? UserId { get; }

        public string Name { get; }
    }
}
