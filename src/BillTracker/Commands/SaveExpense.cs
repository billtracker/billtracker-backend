using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public class SaveExpense
    {
        public const string ExpenseTypeNotFound = "Expense type does not exist.";

        private readonly BillTrackerContext _context;

        public SaveExpense(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<ExpenseModel>> Handle(SaveExpenseParameters input)
        {
            if (!await _context.DoesExist<User>(input.UserId))
            {
                return CommonErrors.UserNotExist;
            }

            if (input.ExpenseTypeId.HasValue && !await _context.DoesExist<ExpenseType>(input.ExpenseTypeId.Value))
            {
                return ExpenseTypeNotFound;
            }

            if (input.ExpenseId.HasValue)
            {
                var updated = await UpdateExpense(input);
                if (updated.IsError)
                {
                    return updated.Error;
                }

                return updated.Result?.ToModel();
            }

            var created = await CreateExpense(input);
            if (created.IsError)
            {
                return created.Error;
            }

            return created.Result.ToModel();
        }

        private async Task<ResultOrError<Expense>> UpdateExpense(SaveExpenseParameters input)
        {
            var expense = await _context.Expenses.SingleOrDefaultAsync(x => x.Id == input.ExpenseId);
            if (expense is null)
            {
                return (Expense)null;
            }

            expense.Update(input.Name, input.Price, input.Amount, input.ExpenseTypeId);
            await _context.SaveChangesAsync();

            return expense;
        }

        private async Task<ResultOrError<Expense>> CreateExpense(SaveExpenseParameters input)
        {
            if (!await _context.ExpensesAggregates.AnyAsync(x => x.Id == input.AggregateId))
            {
                return CommonErrors.ExpenseAggregateNotFound;
            }

            var expense = Expense.Create(input.Name, input.Price, input.Amount, input.AggregateId, input.ExpenseTypeId);
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return expense;
        }
    }

    public class SaveExpenseParameters
    {
        public SaveExpenseParameters(
            Guid userId,
            Guid aggregateId,
            string name,
            decimal price,
            int amount = 1,
            Guid? expenseId = null,
            Guid? expenseTypeId = null)
        {
            UserId = userId;
            AggregateId = aggregateId;
            Name = name;
            Price = price;
            Amount = amount;
            ExpenseId = expenseId;
            ExpenseTypeId = expenseTypeId;
        }

        public Guid UserId { get; }

        public Guid AggregateId { get; }

        public Guid? ExpenseId { get; }

        public string Name { get; }

        public decimal Price { get; }

        public int Amount { get; }

        public Guid? ExpenseTypeId { get; }
    }
}
