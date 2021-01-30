using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    internal class BillTrackerContext : DbContext
    {
        public BillTrackerContext(DbContextOptions<BillTrackerContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(t =>
            {
                t.HasOne(p => p.User)
                .WithOne(u => u.RefreshToken)
                .HasForeignKey<RefreshToken>(x => x.UserId);

                t.HasAlternateKey(x => x.Token);
            });
        }
    }
}
