using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class Expense : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public Guid UserId { get; private set; }

        public User User { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public DateTimeOffset AddedDate { get; private set; }

        [Required]
        public decimal Amount { get; private set; }

        [Required]
        public Guid ExpenseTypeId { get; private set; }

        public ExpenseType ExpenseType { get; private set; }

        public static Expense Create(
            Guid userId,
            string name,
            decimal amount,
            DateTimeOffset addedAt,
            Guid expenseTypeId) => new Expense
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Amount = amount,
                AddedDate = addedAt,
                ExpenseTypeId = expenseTypeId,
            };
    }
}
