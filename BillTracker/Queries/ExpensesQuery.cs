using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Queries
{
    public class ExpensesQuery
    {
        private readonly BillTrackerContext _context;

        public ExpensesQuery(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ExpenseModel> GetById(Guid id)
        {
            var result = await _context.Expenses
                .Include(x => x.Aggregate)
                .Include(x => x.ExpenseType)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
            return result == null ? null : new ExpenseModel(result);
        }

        public async Task<ResultOrError<PagedResult<ExpenseModel>>> GetMany(Guid userId, int pageNumber, int pageSize = 50, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            if (!await _context.DoesExist<User>(userId))
            {
                return CommonErrors.UserNotExist;
            }

            var baseQuery = _context.Expenses
                .Include(x => x.Aggregate)
                .Include(x => x.ExpenseType)
                .Where(x => x.Aggregate.UserId == userId);

            if (fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Aggregate.AddedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Aggregate.AddedDate <= toDate.Value);
            }

            var totalItems = await baseQuery.CountAsync();
            var items = totalItems == 0
                ? new List<ExpenseModel>()
                : await baseQuery.OrderByDescending(x => x.Aggregate.AddedDate)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(x => new ExpenseModel(x))
                                 .ToListAsync();

            return ResultOrError<PagedResult<ExpenseModel>>.FromResult(
                new PagedResult<ExpenseModel>(items, totalItems));
        }

        public async Task<ExpenseAggregateModel> GetExpensesAggregate(Guid aggregateId)
        {
            var result = await _context.ExpensesAggregates
                .Include(x => x.Expenses)
                .ThenInclude(x => x.ExpenseType)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == aggregateId);
            return result == null ? null : new ExpenseAggregateModel(result);
        }

        public async Task<ResultOrError<PagedResult<ExpenseAggregateModel>>> GetManyExpensesAggregate(Guid userId, int pageNumber, int pageSize = 50, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            if (!await _context.DoesExist<User>(userId))
            {
                return CommonErrors.UserNotExist;
            }

            var baseQuery = _context.ExpensesAggregates
                .Include(x => x.Expenses)
                .ThenInclude(x => x.ExpenseType)
                .Where(x => x.UserId == userId);

            if (fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedDate <= toDate.Value);
            }

            var totalItems = await baseQuery.CountAsync();
            var items = totalItems == 0
                ? new List<ExpenseAggregateModel>()
                : await baseQuery.OrderByDescending(x => x.AddedDate)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(x => new ExpenseAggregateModel(x))
                                 .ToListAsync();

            return ResultOrError<PagedResult<ExpenseAggregateModel>>.FromResult(
                new PagedResult<ExpenseAggregateModel>(items, totalItems));
        }
    }
}
