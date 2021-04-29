using System;
using System.Collections.Generic;
using System.Linq;

namespace BillTracker.Models
{
    public record ExpenseAggregateModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public Guid UserId { get; init; }

        public bool IsDraft { get; init; }

        public decimal TotalExpensesPrice
        {
            get
            {
                return Expenses.Sum(x => x.Price * x.Amount);
            }
        }

        public IEnumerable<ExpenseModel> Expenses { get; init; }

        public IEnumerable<ExpenseBillFileModel> Bills { get; init; }
    }
}
