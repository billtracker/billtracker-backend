using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Expenses
{
    public interface IExpensesQuery
    {
        Task<ExpenseModel> GetById(Guid id);

        Task<ResultOrError<IEnumerable<ExpenseModel>>> GetByUserId(Guid userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null);
    }

    internal class ExpensesQuery : IExpensesQuery
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

        public async Task<ResultOrError<IEnumerable<ExpenseModel>>> GetByUserId(Guid userId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return CommonErrors.UserNotExist;
            }

            var baseQuery = _context.Expenses.Where(x => x.AddedById == userId);

            if (fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedAt <= toDate.Value);
            }

            var result = await baseQuery
                .Select(x => new ExpenseModel(x))
                .ToListAsync();
            return ResultOrError<IEnumerable<ExpenseModel>>.FromResult(result);
        }
    }
}
