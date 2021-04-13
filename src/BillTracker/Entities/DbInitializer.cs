using BillTracker.Modules;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    internal class DbInitializer : IInitializable
    {
        private readonly BillTrackerContext _context;

        public DbInitializer(BillTrackerContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.Migrate();
        }
    }
}
