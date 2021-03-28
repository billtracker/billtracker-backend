using System;
using System.Collections.Generic;
using System.Linq;
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
            IsDraft = entity.IsDraft;
            Expenses = entity.Expenses.Select(x => new ExpenseModel(x));
            Bills = entity.ExpenseBillFiles.Select(x => new ExpenseBillFileModel(x));
        }

        public Guid Id { get; }

        public string Name { get; }

        public Guid UserId { get; }

        public bool IsDraft { get; }

        public decimal TotalAmount
        {
            get
            {
                return Expenses.Sum(x => x.Amount);
            }
        }

        public IEnumerable<ExpenseModel> Expenses { get; }

        public IEnumerable<ExpenseBillFileModel> Bills { get; }
    }
}
