using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    internal static class UsersDbSetExtensions
    {
        public static Task<bool> DoesExist(this DbSet<User> dbSet, Guid userId)
        {
            return dbSet.AnyAsync(x => x.Id == userId);
        }
    }
}
