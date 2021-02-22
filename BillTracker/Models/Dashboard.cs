using System;
using System.Collections.Generic;

namespace BillTracker.Models
{
    public class Dashboard
    {
        public static readonly Dashboard Empty = new Dashboard(MetricsModel.Empty);

        public Dashboard(MetricsModel metrics)
        {
            Metrics = metrics;
        }

        public MetricsModel Metrics { get; }

        public IEnumerable<CalendarDayModel> Calendar { get; }

        public class MetricsModel
        {
            public static readonly MetricsModel Empty = new MetricsModel(0, 0, null);

            public MetricsModel(decimal total, int totalTransfers, ExpenseModel mostExpensive)
            {
                Total = total;
                TotalTransfers = totalTransfers;
                MostExpensive = mostExpensive;
            }

            public decimal Total { get; }
            public int TotalTransfers { get; }
            public ExpenseModel MostExpensive { get; }
        }

        public class CalendarDayModel
        {
            public CalendarDayModel(DateTimeOffset calendarDay, IEnumerable<ExpenseModel> expenses)
            {
                Day = calendarDay;
                Expenses = expenses;
            }

            public DateTimeOffset Day { get; }

            public IEnumerable<ExpenseModel> Expenses { get; }
        }
    }
}
