using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ResultOrError<ExpenseModel>> Handle(AddExpenseParameters input)
        {
            if (!await _context.DoesExist<User>(input.UserId))
            {
                return CommonErrors.UserNotExist;
            }

            if (input.ExpenseTypeId.HasValue && !await _context.DoesExist<ExpenseType>(input.ExpenseTypeId.Value))
            {
                return ExpenseTypeNotExist;
            }

            Expense newExpense;
            var aggregateId = input.AggregateId;
            if (aggregateId.HasValue)
            {
                if (!await _context.ExpensesAggregates.AnyAsync(x => x.Id == input.AggregateId.Value))
                {
                    return CommonErrors.ExpenseAggregateNotFound;
                }
            }
            else
            {
                var aggregate = ExpensesAggregate.Create(input.UserId, input.Name, input.AddedDate, isDraft: false);
                await _context.ExpensesAggregates.AddAsync(aggregate);
                aggregateId = aggregate.Id;
            }

            newExpense = Expense.Create(input.Name, input.Amount, aggregateId.Value, input.ExpenseTypeId);
            await _context.Expenses.AddAsync(newExpense);
            await _context.SaveChangesAsync();

            return new ExpenseModel(newExpense);
        }
    }

    public class AddExpenseParameters
    {
        public AddExpenseParameters(
            Guid userId,
            string name,
            decimal amount,
            DateTimeOffset? addedDate = null,
            Guid? aggregateId = null,
            Guid? expenseTypeId = null)
        {
            UserId = userId;
            Name = name;
            Amount = amount;
            ExpenseTypeId = expenseTypeId;
            AddedDate = addedDate ?? DateTimeOffset.Now;
            AggregateId = aggregateId;
        }

        public Guid UserId { get; }

        public string Name { get; }

        public decimal Amount { get; }

        public Guid? ExpenseTypeId { get; }

        public DateTimeOffset AddedDate { get; }

        public Guid? AggregateId { get; }
    }
}
