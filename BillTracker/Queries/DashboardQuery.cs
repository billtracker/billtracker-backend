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

            var stats = await GetStatisticsFilteredByDate(userId, fromDate, toDate);
            var calendar = await GetCalendar(userId);

            return new Dashboard(stats.Metrics, calendar, stats.ExpenseTypes);
        }

        private static async Task<MetricsModel> GetMetrics(IQueryable<Expense> baseQuery)
        {
            var mostExpensive = await baseQuery
                .OrderByDescending(x => x.Amount)
                .Select(x => new ExpenseModel(x))
                .FirstOrDefaultAsync();

            var peakStats = await baseQuery
                .GroupBy(
                    x => x.UserId,
                    (key, expenses) => new
                    {
                        Total = expenses.Sum(x => x.Amount),
                        Transfers = expenses.Count(),
                    })
                .SingleOrDefaultAsync();

            return new MetricsModel(
                total: peakStats?.Total ?? default,
                transfers: peakStats?.Transfers ?? default,
                mostExpensive: mostExpensive);
        }

        private static async Task<IReadOnlyList<DashboardExpenseTypeModel>> GetExpenseTypes(IQueryable<Expense> baseQuery)
        {
            var result = await baseQuery
                .GroupBy(
                    x => new { Id = x.ExpenseTypeId, x.ExpenseType.Name },
                    (key, types) => new DashboardExpenseTypeModel(
                        key.Id,
                        key.Name,
                        types.Sum(x => x.Amount)))
                .ToListAsync();

            return result;
        }

        private async Task<(MetricsModel Metrics, IReadOnlyList<DashboardExpenseTypeModel> ExpenseTypes)> GetStatisticsFilteredByDate(
            Guid userId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var baseQuery = _context.Expenses
                .Include(x => x.ExpenseType)
                .Where(
                    x => x.UserId == userId &&
                         (!fromDate.HasValue || x.AddedDate >= fromDate.Value) &&
                         (!toDate.HasValue || x.AddedDate <= toDate.Value));

            var metrics = await GetMetrics(baseQuery);
            var expenseTypes = await GetExpenseTypes(baseQuery);

            return (metrics, expenseTypes);
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
