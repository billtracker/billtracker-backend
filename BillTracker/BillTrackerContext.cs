using BillTracker.Users;
using Microsoft.EntityFrameworkCore;

namespace BillTracker
{
    internal class BillTrackerContext : DbContext
    {
        public BillTrackerContext(DbContextOptions<BillTrackerContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
    }
}
