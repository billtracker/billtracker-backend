using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public decimal Price { get; private set; }

        [Required]
        public DateTimeOffset AddedDate { get; private set; }

        public IEnumerable<Expense> Expenses { get; private set; }

        [Required]
        public bool IsDraft { get; private set; }

        public IEnumerable<ExpenseBillFile> ExpenseBillFiles { get; private set; }

        public static ExpensesAggregate Create(
            Guid userId,
            string name,
            decimal price,
            DateTimeOffset addedDate,
            bool isDraft) => new ExpensesAggregate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Price = price,
                AddedDate = addedDate,
                IsDraft = isDraft,
                Expenses = new List<Expense>(),
                ExpenseBillFiles = new List<ExpenseBillFile>(),
            };

        public void Update(
            string name = null,
            decimal? price = null,
            DateTimeOffset? addedDate = null,
            bool? isDraft = null)
        {
            Name = string.IsNullOrEmpty(name) ? Name : name;
            Price = price ?? Price;
            AddedDate = addedDate ?? AddedDate;
            IsDraft = isDraft ?? IsDraft;
        }
    }
}
