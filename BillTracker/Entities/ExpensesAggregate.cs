using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class ExpensesAggregate : IEntity
    {
        public Guid Id { get; private set; }

        [Required]
        public Guid UserId { get; private set; }

        public User User { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public DateTimeOffset AddedDate { get; private set; }

        public ICollection<Expense> Expenses { get; private set; }

        public static ExpensesAggregate Create(
            Guid userId,
            string name,
            DateTimeOffset addedDate) => new ExpensesAggregate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                AddedDate = addedDate,
            };

        public void Update(
            string name = null,
            DateTimeOffset? addedDate = null)
        {
            Name = string.IsNullOrEmpty(name) ? Name : name;
            AddedDate = addedDate ?? AddedDate;
        }
    }
}
