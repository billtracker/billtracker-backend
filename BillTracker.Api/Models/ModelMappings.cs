using System.Collections.Generic;
using System.Linq;
using BillTracker.Expenses;

namespace BillTracker.Api.Models
{
    public static class ModelMappings
    {
        public static DashboardResponse MapExpensesToDashboardResponse(IEnumerable<ExpenseModel> expenses)
        {
            if (expenses != null && expenses.Any())
            {
                return new DashboardResponse(
                    total: expenses.Sum(x => x.Amount),
                    mostExpensive: expenses.OrderByDescending(x => x.Amount).First(),
                    totalTransfers: expenses.Count());
            }

            return new DashboardResponse(0, null, 0);
        }
    }
}
