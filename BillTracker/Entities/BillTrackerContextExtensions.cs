using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Entities
{
    internal static class BillTrackerContextExtensions
    {
        public static Task<bool> DoesExist<TEntity>(this BillTrackerContext context, Guid entityId)
            where TEntity : class, IEntity
        {
            return context.Set<TEntity>().AnyAsync(x => x.Id == entityId);
        }
    }
}
