using System;
using System.Threading.Tasks;
using BillTracker.Entities;
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

        public async Task<ResultOrError<Guid>> Handle(SaveExpenseAggregateParameters parameters)
        {
            if (parameters.Id.HasValue)
            {
                return await Update(parameters);
            }

            var result = await Create(parameters);
            return result;
        }

        private async Task<Guid> Create(SaveExpenseAggregateParameters parameters)
        {
            var aggregate = ExpensesAggregate.Create(parameters.UserId, parameters.Name, parameters.Price, parameters.AddedDate, parameters.IsDraft);
            await _context.ExpensesAggregates.AddAsync(aggregate);
            await _context.SaveChangesAsync();
            return aggregate.Id;
        }

        private async Task<ResultOrError<Guid>> Update(SaveExpenseAggregateParameters parameters)
        {
            var aggregate = await _context.ExpensesAggregates.SingleOrDefaultAsync(x => x.Id == parameters.Id.Value);
            if (aggregate == null)
            {
                return CommonErrors.ExpenseAggregateNotFound;
            }

            aggregate.Update(
                name: parameters.Name,
                addedDate: parameters.AddedDate,
                isDraft: parameters.IsDraft);
            await _context.SaveChangesAsync();

            return aggregate.Id;
        }
    }

    public class SaveExpenseAggregateParameters
    {
        public SaveExpenseAggregateParameters(Guid? id, Guid userId, string name, decimal price, DateTimeOffset? addedDate = null, bool isDraft = false)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Price = price;
            AddedDate = addedDate ?? DateTimeOffset.Now;
            IsDraft = isDraft;
        }

        public Guid? Id { get; }

        public Guid UserId { get; }

        public string Name { get; }

        public decimal Price { get; }

        public DateTimeOffset AddedDate { get; }

        public bool IsDraft { get; }
    }
}
