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
            var result = await _context.Expenses.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            return result == null ? null : new ExpenseModel(result);
        }

        public async Task<ResultOrError<PagedResult<ExpenseModel>>> GetMany(Guid userId, int pageNumber, int pageSize = 50, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!userExists)
            {
                return CommonErrors.UserNotExist;
            }

            var baseQuery = _context.Expenses.Include(x => x.ExpenseType).Where(x => x.UserId == userId);

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
                ? new List<ExpenseModel>()
                : await baseQuery.OrderByDescending(x => x.AddedDate)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(x => new ExpenseModel(x))
                                 .ToListAsync();

            return ResultOrError<PagedResult<ExpenseModel>>.FromResult(new PagedResult<ExpenseModel>(items, totalItems));
        }
    }
}
