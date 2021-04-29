using System;

namespace BillTracker.Entities
{
    internal class DashboardCalendarDayView
    {
        public Guid UserId { get; private set; }

        public DateTime AddedDate { get; private set; }

        public decimal TotalPrice { get; private set; }
    }
}
