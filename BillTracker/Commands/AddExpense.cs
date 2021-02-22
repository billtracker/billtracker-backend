using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public interface IAddExpense
    {
        Task<ResultOrError<ExpenseModel>> Handle(AddExpenseParameters input);
    }

    internal class AddExpense : IAddExpense
    {
        private readonly BillTrackerContext _context;

        public AddExpense(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<ExpenseModel>> Handle(AddExpenseParameters input)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == input.UserId);
            if (!userExists)
            {
                return CommonErrors.UserNotExist;
            }

            var expense = Expense.Create(input.UserId, input.Name, input.Amount, input.AddedAt);
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return new ExpenseModel(expense);
        }
    }

    public class AddExpenseParameters
    {
        public AddExpenseParameters(Guid userId, string name, decimal amount, DateTimeOffset? addedAt = null)
        {
            UserId = userId;
            Name = name;
            Amount = amount;
            AddedAt = addedAt ?? DateTimeOffset.Now;
        }

        public Guid UserId { get; }
        public string Name { get; }
        public decimal Amount { get; }
        public DateTimeOffset AddedAt { get; }
    }
}
