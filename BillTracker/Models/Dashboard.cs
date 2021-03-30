using System;
using System.Collections.Generic;

namespace BillTracker.Models
{
    public class Dashboard
    {
        public Dashboard(
            MetricsModel metrics,
            IReadOnlyList<CalendarDayModel> calendar,
            IReadOnlyList<DashboardExpenseTypeModel> expenseTypes)
        {
            Metrics = metrics;
            Calendar = calendar;
            ExpenseTypes = expenseTypes;
        }

        public MetricsModel Metrics { get; }

        public IReadOnlyList<CalendarDayModel> Calendar { get; }

        public IReadOnlyList<DashboardExpenseTypeModel> ExpenseTypes { get; }

        public class MetricsModel
        {
            public MetricsModel(decimal total, int transfers, ExpenseModel mostExpensive)
            {
                Total = total;
                Tranfers = transfers;
                MostExpensive = mostExpensive;
            }

            public decimal Total { get; }

            public int Tranfers { get; }

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

        public class DashboardExpenseTypeModel
        {
            public DashboardExpenseTypeModel(Guid? expenseTypeId, string expenseTypeName, decimal total)
            {
                ExpenseTypeId = expenseTypeId;
                ExpenseTypeName = expenseTypeName;
                Total = total;
            }

            public Guid? ExpenseTypeId { get; }

            public string ExpenseTypeName { get; }

            public decimal Total { get; }
        }
    }
}
