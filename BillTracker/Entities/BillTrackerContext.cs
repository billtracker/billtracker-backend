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

        internal virtual DbSet<ExpenseType> ExpenseTypes { get; set; }

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
                expense.HasOne(x => x.User)
                       .WithMany(x => x.Expenses)
                       .HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<ExpenseType>(type =>
            {
                type.HasMany(x => x.Expenses)
                    .WithOne(x => x.ExpenseType)
                    .HasForeignKey(x => x.ExpenseTypeId);
            });

            modelBuilder.Entity<DashboardCalendarDayView>(calendarDay =>
            {
                // Created raw sql just becausee DateTimeOffset is not translatable yet.
                calendarDay.HasNoKey()
                           .ToView(null)
                           .ToSqlQuery($@"
SELECT
    ""{nameof(Expense.UserId)}"" AS ""{nameof(DashboardCalendarDayView.AddedById)}"",
    SUM(""{nameof(Expense.Amount)}"") AS ""{nameof(DashboardCalendarDayView.TotalAmount)}"",
    ""{nameof(Expense.AddedDate)}""::DATE AS ""{nameof(DashboardCalendarDayView.AddedAt)}""
FROM ""{nameof(Expenses)}""
GROUP BY 
    ""{nameof(Expense.UserId)}"",
    ""{nameof(Expense.AddedDate)}""::DATE");
            });
        }
    }
}
