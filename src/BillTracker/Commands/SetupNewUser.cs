using System;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public class SetupNewUser
    {
        private readonly BillTrackerContext _context;

        public SetupNewUser(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<SuccessOrError> Handle(Guid userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return CommonErrors.UserNotExist;
            }

            if (user.WasSetup)
            {
                return SuccessOrError.FromSuccess();
            }

            await InitializeDefaultExpenseTypes(user.Id);
            user.Setup();
            await _context.SaveChangesAsync();

            return SuccessOrError.FromSuccess();
        }

        private async Task InitializeDefaultExpenseTypes(Guid userId)
        {
            var defaultExpenseTypes = await _context.ExpenseTypes.Where(x => x.IsDefault).ToListAsync();
            if (defaultExpenseTypes != null && defaultExpenseTypes.Any())
            {
                _context.ExpenseTypes.AddRange(
                    defaultExpenseTypes.Select(x => ExpenseType.Create(userId, x.Name)));
            }
        }
    }
}
