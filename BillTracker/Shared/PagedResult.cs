using System.Collections.Generic;

namespace BillTracker.Shared
{
    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> items, int totalItems)
        {
            Items = items;
            TotalItems = totalItems;
        }

        public IEnumerable<T> Items { get; }

        public int TotalItems { get; }
    }
}
