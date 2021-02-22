﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BillTracker.Entities
{
    internal class Expense
    {
        public Guid Id { get; private set; }

        [Required]
        public Guid AddedById { get; private set; }

        public User AddedBy { get; private set; }

        [Required]
        public string Name { get; private set; }

        [Required]
        public DateTimeOffset AddedAt { get; private set; }

        [Required]
        public decimal Amount { get; private set; }

        public static Expense Create(
            Guid userId,
            string name,
            decimal amount,
            DateTimeOffset addedAt) => new Expense
            {
                Id = Guid.NewGuid(),
                AddedById = userId,
                Name = name,
                Amount = amount,
                AddedAt = addedAt,
            };
    }
}
