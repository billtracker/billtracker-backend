using System;
using System.Collections.Generic;
using System.Linq;
using BillTracker.Entities;

namespace BillTracker.Models
{
    public record ExpenseAggregateModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public Guid UserId { get; init; }

        public bool IsDraft { get; init; }

        public decimal TotalAmount
        {
            get
            {
                return Expenses.Sum(x => x.Amount);
            }
        }

        public IEnumerable<ExpenseModel> Expenses { get; init; }

        public IEnumerable<ExpenseBillFileModel> Bills { get; init; }
    }
}
