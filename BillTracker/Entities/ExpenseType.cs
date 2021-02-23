using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class ExpenseType : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public bool IsBuiltIn { get; private set; }

        public Guid? UserId { get; private set; }

        public User User { get; private set; }

        public IEnumerable<Expense> Expenses { get; private set; }

        public static ExpenseType Create(
            Guid userId,
            string name) => new ExpenseType
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                IsBuiltIn = false,
            };
    }
}
