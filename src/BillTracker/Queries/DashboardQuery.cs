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

            return new Dashboard
            {
                Metrics = stats.Metrics,
                ExpenseTypes = stats.ExpenseTypes,
                Calendar = calendar,
            };
        }

        private static async Task<MetricsModel> GetMetrics(IQueryable<Expense> baseQuery)
        {
            var mostExpensive = await baseQuery
                .OrderByDescending(x => x.Price * x.Amount)
                .Select(x => x.ToModel())
                .FirstOrDefaultAsync();

            var peakStats = await baseQuery
                .GroupBy(
                    x => x.Aggregate.UserId,
                    (key, expenses) => new
                    {
                        Total = expenses.Sum(x => x.Price * x.Amount),
                        Transfers = expenses.Count(),
                    })
                .SingleOrDefaultAsync();

            return new MetricsModel
            {
                Total = peakStats?.Total ?? default,
                Tranfers = peakStats?.Transfers ?? default,
                MostExpensive = mostExpensive,
            };
        }

        private static async Task<IReadOnlyList<DashboardExpenseTypeModel>> GetExpenseTypes(IQueryable<Expense> baseQuery)
        {
            var result = await baseQuery
                .GroupBy(
                    x => new { Id = x.ExpenseTypeId, x.ExpenseType.Name },
                    (key, types) => new DashboardExpenseTypeModel
                    {
                        ExpenseTypeId = key.Id,
                        ExpenseTypeName = key.Name,
                        Total = types.Sum(x => x.Price * x.Amount),
                    })
                .ToListAsync();

            return result;
        }

        private async Task<(MetricsModel Metrics, IReadOnlyList<DashboardExpenseTypeModel> ExpenseTypes)> GetStatisticsFilteredByDate(
            Guid userId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var baseQuery = _context.Expenses
                .Include(x => x.ExpenseType)
                .Include(x => x.Aggregate)
                .Where(
                    x => x.Aggregate.UserId == userId &&
                         !x.Aggregate.IsDraft &&
                         (!fromDate.HasValue || x.Aggregate.AddedDate >= fromDate.Value) &&
                         (!toDate.HasValue || x.Aggregate.AddedDate <= toDate.Value));

            var metrics = await GetMetrics(baseQuery);
            var expenseTypes = await GetExpenseTypes(baseQuery);

            return (metrics, expenseTypes);
        }

        private async Task<IReadOnlyList<CalendarDayModel>> GetCalendar(Guid userId)
        {
            var result = await _context.DashboardCalendarDays
                .Where(x => x.UserId == userId)
                .Select(x => new CalendarDayModel
                {
                    Day = x.AddedDate,
                    Total = x.TotalPrice,
                })
                .ToListAsync();

            return result;
        }
    }
}
