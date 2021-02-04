using BillTracker.Expenses;

namespace BillTracker.Api.Models
{
    public class DashboardResponse
    {
        public DashboardResponse(decimal total, ExpenseModel mostExpensive, int totalTransfers)
        {
            Total = total;
            MostExpensive = mostExpensive;
            TotalTransfers = totalTransfers;
        }

        public decimal Total { get; }
        public ExpenseModel MostExpensive { get; }
        public int TotalTransfers { get; }
    }
}
