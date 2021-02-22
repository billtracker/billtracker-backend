using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    internal class BillTrackerContext : DbContext
    {
        public BillTrackerContext(DbContextOptions<BillTrackerContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<DashboardCalendarDayView> DashboardCalendarDays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(t =>
            {
                t.HasOne(p => p.User)
                .WithOne(u => u.RefreshToken)
                .HasForeignKey<RefreshToken>(x => x.UserId);

                t.HasAlternateKey(x => x.Token);
            });

            modelBuilder.Entity<Expense>(e =>
            {
                e.HasOne(x => x.AddedBy)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.AddedById);
            });

            modelBuilder.Entity<DashboardCalendarDayView>(x =>
            {
                x.HasNoKey();
                x.ToSqlQuery($@"
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
