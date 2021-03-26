using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class Expense : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public decimal Amount { get; private set; }

        [Required]
        public Guid ExpenseTypeId { get; private set; }

        public ExpenseType ExpenseType { get; private set; }

        public Guid AggregateId { get; private set; }

        public ExpensesAggregate Aggregate { get; private set; }

        public static Expense Create(
            string name,
            decimal amount,
            Guid expenseTypeId,
            Guid aggregateId) => new Expense
            {
                Id = Guid.NewGuid(),
                Name = name,
                Amount = amount,
                ExpenseTypeId = expenseTypeId,
                AggregateId = aggregateId,
            };
    }
}
