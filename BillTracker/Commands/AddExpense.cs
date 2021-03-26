using System;
using System.Threading.Tasks;
using BillTracker.Entities;
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

            Expense newExpense;
            var aggregateId = input.AggregateId;
            if (aggregateId.HasValue)
            {
                if (!await _context.ExpensesAggregates.AnyAsync(x => x.Id == input.AggregateId.Value))
                {
                    return CommonErrors.ExpenseAggregateDoesNotExist;
                }
            }
            else
            {
                var aggregate = ExpensesAggregate.Create(input.UserId, input.Name, input.AddedDate, isDraft: false);
                await _context.ExpensesAggregates.AddAsync(aggregate);
                aggregateId = aggregate.Id;
            }

            newExpense = Expense.Create(input.Name, input.Amount, input.ExpenseTypeId, aggregateId.Value);
            await _context.Expenses.AddAsync(newExpense);
            await _context.SaveChangesAsync();

            return newExpense.Id;
        }
    }

    public class AddExpenseParameters
    {
        public AddExpenseParameters(
            Guid userId, string name, decimal amount, Guid expenseTypeId, DateTimeOffset? addedDate = null, Guid? aggregateId = null)
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

        public Guid ExpenseTypeId { get; }

        public DateTimeOffset AddedDate { get; }

        public Guid? AggregateId { get; }
    }
}
