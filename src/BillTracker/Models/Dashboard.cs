using System;
using System.Collections.Generic;

namespace BillTracker.Models
{
    public record Dashboard
    {
        public MetricsModel Metrics { get; init; }

        public IReadOnlyList<CalendarDayModel> Calendar { get; init; }

        public IReadOnlyList<DashboardExpenseTypeModel> ExpenseTypes { get; init; }

        public record MetricsModel
        {
            public decimal Total { get; init; }

            public int Tranfers { get; init; }

            public ExpenseModel MostExpensive { get; init; }
        }

        public record CalendarDayModel
        {
            public DateTime Day { get; init; }

            public decimal Total { get; init; }
        }

        public record DashboardExpenseTypeModel
        {
            public Guid? ExpenseTypeId { get; init; }

            public string ExpenseTypeName { get; init; }

            public decimal Total { get; init; }
        }
    }
}
