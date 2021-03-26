﻿using System;
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
                ? new List<ExpensesAggregate>()
                : await baseQuery.OrderByDescending(x => x.AddedDate)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .AsNoTracking()
                                 .ToListAsync();

            var resultItems = items
                .SelectMany(x => x.Expenses)
                .Select(x => new ExpenseModel(x));
            return ResultOrError<PagedResult<ExpenseModel>>.FromResult(
                new PagedResult<ExpenseModel>(resultItems, totalItems));
        }
    }
}
