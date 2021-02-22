using System;

namespace BillTracker.Entities
{
    internal class DashboardCalendarDayView
    {
        public Guid AddedById { get; private set; }

        public DateTime AddedAt { get; private set; }

        public decimal TotalAmount { get; private set; }
    }
}
