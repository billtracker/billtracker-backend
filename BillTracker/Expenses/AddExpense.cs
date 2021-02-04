using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Expenses
{
    internal class AddExpense
        : IHandle<AddExpenseParameters, ResultOrError<AddExpenseResult>>
    {
        private readonly BillTrackerContext _context;

        public AddExpense(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<AddExpenseResult>> Handle(AddExpenseParameters input)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == input.UserId);
            if (!userExists)
            {
                return AddExpenseErrors.UserNotExist;
            }

            var expense = Expense.Create(input.UserId, input.Amount, input.AddedAt);
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return new AddExpenseResult(expense);
        }
    }

    public class AddExpenseParameters
    {
        public AddExpenseParameters(Guid userId, decimal amount, DateTimeOffset? addedAt = null)
        {
            UserId = userId;
            Amount = amount;
            AddedAt = addedAt ?? DateTimeOffset.Now;
        }

        public Guid UserId { get; }

        public decimal Amount { get; }

        public DateTimeOffset AddedAt { get; }
    }

    public class AddExpenseResult
    {
        internal AddExpenseResult(Expense expense)
        {
            Id = expense.Id;
            AddedById = expense.AddedById;
            AddedAt = expense.AddedAt;
            Amount = expense.Amount;
        }

        public Guid Id { get; }
        public Guid AddedById { get; }
        public DateTimeOffset AddedAt { get; }
        public decimal Amount { get; }
    }

    public static class AddExpenseErrors
    {
        public const string UserNotExist = "User does not exist.";
    }
}
