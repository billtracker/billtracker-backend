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

        Task<ResultOrError<IEnumerable<ExpenseModel>>> GetByUserId(Guid userId);
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

        public async Task<ResultOrError<IEnumerable<ExpenseModel>>> GetByUserId(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.Expenses)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return CommonErrors.UserNotExist;
            }

            var result = user.Expenses.Select(x => new ExpenseModel(x));
            return ResultOrError<IEnumerable<ExpenseModel>>.FromResult(result);
        }
    }
}
