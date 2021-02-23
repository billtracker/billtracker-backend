using System;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

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

            var baseQuery = _context.Expenses.Where(x => x.AddedById == userId);

            var calendar = await _context.DashboardCalendarDays.Where(x => x.AddedById == userId).ToListAsync();

            if (fromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AddedAt <= toDate.Value);
            }

            var metrics = await baseQuery
                .GroupBy(
                    x => x.AddedById,
                    (key, expenses) => new Dashboard.MetricsModel(
                        expenses.Sum(x => x.Amount),
                        expenses.Count(),
                        new ExpenseModel(baseQuery.OrderByDescending(x => x.Amount).First())))
                .SingleOrDefaultAsync();

            return new Dashboard(
                metrics,
                calendar.Select(x => new Dashboard.CalendarDayModel(x.AddedAt, x.TotalAmount)));
        }
    }
}
