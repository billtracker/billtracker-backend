using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public class SaveExpenseAggregate
    {
        private readonly BillTrackerContext _context;

        public SaveExpenseAggregate(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<ExpenseAggregateModel>> Handle(SaveExpenseAggregateParameters parameters)
        {
            if (parameters.Id.HasValue)
            {
                return await Update(parameters);
            }

            var result = await Create(parameters);
            return result;
        }

        private async Task<ExpenseAggregateModel> Create(SaveExpenseAggregateParameters parameters)
        {
            var aggregate = ExpensesAggregate.Create(parameters.UserId, parameters.Name, parameters.AddedDate);
            await _context.ExpensesAggregates.AddAsync(aggregate);
            await _context.SaveChangesAsync();
            return new ExpenseAggregateModel(aggregate);
        }

        private async Task<ResultOrError<ExpenseAggregateModel>> Update(SaveExpenseAggregateParameters parameters)
        {
            var aggregate = await _context.ExpensesAggregates.SingleOrDefaultAsync(x => x.Id == parameters.Id.Value);
            if (aggregate == null)
            {
                return CommonErrors.ExpenseAggregateDoesNotExist;
            }

            aggregate.Update(name: parameters.Name);
            await _context.SaveChangesAsync();

            return new ExpenseAggregateModel(aggregate);
        }
    }

    public class SaveExpenseAggregateParameters
    {
        public SaveExpenseAggregateParameters(Guid? id, Guid userId, string name, DateTimeOffset? addedDate = null)
        {
            Id = id;
            UserId = userId;
            Name = name;
            AddedDate = addedDate ?? DateTimeOffset.Now;
        }

        public Guid? Id { get; }

        public Guid UserId { get; }

        public string Name { get; }

        public DateTimeOffset AddedDate { get; }
    }
}
