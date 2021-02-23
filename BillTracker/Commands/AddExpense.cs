using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;

namespace BillTracker.Commands
{
    public class AddExpense
    {
        public const string ExpenseTypeNotExist = "Expense type does not exist.";

        private readonly BillTrackerContext _context;

        public AddExpense(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<Guid>> Handle(AddExpenseParameters input)
        {
            if (!await _context.DoesExist<User>(input.UserId))
            {
                return CommonErrors.UserNotExist;
            }

            if (!await _context.DoesExist<ExpenseType>(input.ExpenseTypeId))
            {
                return ExpenseTypeNotExist;
            }

            var expense = Expense.Create(input.UserId, input.Name, input.Amount, input.AddedDate, input.ExpenseTypeId);
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return expense.Id;
        }
    }

    public class AddExpenseParameters
    {
        public AddExpenseParameters(Guid userId, string name, decimal amount, Guid expenseTypeId, DateTimeOffset? addedDate = null)
        {
            UserId = userId;
            Name = name;
            Amount = amount;
            ExpenseTypeId = expenseTypeId;
            AddedDate = addedDate ?? DateTimeOffset.Now;
        }

        public Guid UserId { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public Guid ExpenseTypeId { get; }

        public DateTimeOffset AddedDate { get; }
    }
}
