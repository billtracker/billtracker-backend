using System;
using System.Collections.Generic;

namespace BillTracker.Models
{
    public class Dashboard
    {
        public Dashboard(MetricsModel metrics, IEnumerable<CalendarDayModel> calendar)
        {
            Metrics = metrics ?? MetricsModel.Empty;
            Calendar = calendar;
        }

        public MetricsModel Metrics { get; }

        public IEnumerable<CalendarDayModel> Calendar { get; }

        public class MetricsModel
        {
            internal static readonly MetricsModel Empty = new MetricsModel(0, 0, null);

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
            public CalendarDayModel(DateTime day, decimal total)
            {
                Day = day;
                Total = total;
            }

            public DateTime Day { get; }

            public decimal Total { get; }
        }
    }
}
