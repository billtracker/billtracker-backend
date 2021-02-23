using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    public class BillTrackerContext : DbContext
    {
        public BillTrackerContext(DbContextOptions<BillTrackerContext> options)
            : base(options)
        {
        }

        internal virtual DbSet<User> Users { get; set; }

        internal virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        internal virtual DbSet<Expense> Expenses { get; set; }

        internal virtual DbSet<DashboardCalendarDayView> DashboardCalendarDays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(token =>
            {
                token.HasOne(p => p.User)
                     .WithOne(u => u.RefreshToken)
                     .HasForeignKey<RefreshToken>(x => x.UserId);

                token.HasAlternateKey(x => x.Token);
            });

            modelBuilder.Entity<Expense>(expense =>
            {
                expense.HasOne(x => x.AddedBy)
                       .WithMany(x => x.Expenses)
                       .HasForeignKey(x => x.AddedById);
            });

            modelBuilder.Entity<DashboardCalendarDayView>(calendarDay =>
            {
                calendarDay.HasNoKey()
                           .ToView(null)
                           .ToSqlQuery($@"
SELECT
    ""{nameof(Expense.AddedById)}"" AS ""{nameof(DashboardCalendarDayView.AddedById)}"",
    SUM(""{nameof(Expense.Amount)}"") AS ""{nameof(DashboardCalendarDayView.TotalAmount)}"",
    ""{nameof(Expense.AddedAt)}""::DATE AS ""{nameof(DashboardCalendarDayView.AddedAt)}""
FROM ""{nameof(Expenses)}""
GROUP BY 
    ""{nameof(Expense.AddedById)}"",
    ""{nameof(Expense.AddedAt)}""::DATE");
            });
        }
    }
}
