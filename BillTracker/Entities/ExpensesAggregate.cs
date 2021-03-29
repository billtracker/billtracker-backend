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

        public IEnumerable<Expense> Expenses { get; private set; } = new List<Expense>();

        [Required]
        public bool IsDraft { get; private set; }

        public IEnumerable<ExpenseBillFile> ExpenseBillFiles { get; private set; } = new List<ExpenseBillFile>();

        public static ExpensesAggregate Create(
            Guid userId,
            string name,
            DateTimeOffset addedDate,
            bool isDraft) => new ExpensesAggregate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                AddedDate = addedDate,
                IsDraft = isDraft,
            };

        public void Update(
            string name = null,
            DateTimeOffset? addedDate = null,
            bool? isDraft = null)
        {
            Name = string.IsNullOrEmpty(name) ? Name : name;
            AddedDate = addedDate ?? AddedDate;
            IsDraft = isDraft ?? IsDraft;
        }
    }
}
