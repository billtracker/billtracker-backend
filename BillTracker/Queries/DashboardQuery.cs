using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;
using static BillTracker.Models.Dashboard;

namespace BillTracker.Queries
{
    public class DashboardQuery
    {
        private readonly BillTrackerContext _context;

        public DashboardQuery(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<Dashboard>> GetDashboard(Guid userId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!userExists)
            {
                return CommonErrors.UserNotExist;
            }

            var baseQuery = _context.Expenses.Where(
                x => x.UserId == userId &&
                     (!fromDate.HasValue || x.AddedDate >= fromDate.Value) &&
                     (!toDate.HasValue || x.AddedDate <= toDate.Value));

            var metrics = await GetMetrics(baseQuery);
            var calendar = await GetCalendar(userId);
            var expenseTypes = await GetExpenseTypes(baseQuery);

            return new Dashboard(metrics, calendar, expenseTypes);
        }

        private static async Task<MetricsModel> GetMetrics(IQueryable<Expense> baseQuery)
        {
            var mostExpensive = await baseQuery
                .Include(x => x.ExpenseType)
                .OrderByDescending(x => x.Amount)
                .Select(x => new ExpenseModel(x))
                .FirstOrDefaultAsync();

            var stats = await baseQuery
                .GroupBy(
                    x => x.UserId,
                    (key, expenses) => new
                    {
                        Total = expenses.Sum(x => x.Amount),
                        Transfers = expenses.Count(),
                    }).SingleOrDefaultAsync();

            return new MetricsModel(
                total: stats?.Total ?? default,
                transfers: stats?.Transfers ?? default,
                mostExpensive: mostExpensive);
        }

        private static async Task<IReadOnlyList<DashboardExpenseTypeModel>> GetExpenseTypes(IQueryable<Expense> baseQuery)
        {
            var result = await baseQuery
                .Include(x => x.ExpenseType)
                .GroupBy(
                    x => new { Id = x.ExpenseTypeId, x.ExpenseType.Name },
                    (key, types) => new DashboardExpenseTypeModel(
                        key.Id,
                        key.Name,
                        types.Sum(x => x.Amount)))
                .ToListAsync();

            return result;
        }

        private async Task<IReadOnlyList<CalendarDayModel>> GetCalendar(Guid userId)
        {
            var result = await _context.DashboardCalendarDays
                .Where(x => x.AddedById == userId)
                .Select(x => new CalendarDayModel(x.AddedAt, x.TotalAmount))
                .ToListAsync();

            return result;
        }
    }
}
