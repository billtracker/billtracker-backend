using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Queries
{
    public class ExpenseTypesQuery
    {
        private readonly BillTrackerContext _context;

        public ExpenseTypesQuery(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpenseTypeModel>> GetAllVisibleForUser(Guid userId)
        {
            var result = await _context.ExpenseTypes
                .Where(x => x.UserId == userId)
                .Select(x => x.ToModel())
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<ExpenseTypeModel>> GetAllDefault()
        {
            var result = await _context.ExpenseTypes
                .Where(x => x.IsDefault)
                .Select(x => x.ToModel())
                .ToListAsync();

            return result;
        }
    }
}
