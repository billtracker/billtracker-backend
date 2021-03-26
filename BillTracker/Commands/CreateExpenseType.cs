using System;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Models;
using BillTracker.Shared;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Commands
{
    public class CreateExpenseType
    {
        public const string ExpenseTypeAlreadyExist = "Expense type already exists.";

        private readonly BillTrackerContext _context;

        public CreateExpenseType(BillTrackerContext context)
        {
            _context = context;
        }

        public async Task<ResultOrError<ExpenseTypeModel>> Handle(CreateExpenseTypeParameters parameters)
        {
            if (!await _context.DoesExist<User>(parameters.UserId))
            {
                return CommonErrors.UserNotExist;
            }

            var expenseTypeExist = await _context.ExpenseTypes.AnyAsync(
                x => x.Name.ToLower() == parameters.Name.ToLower() &&
                     x.UserId == parameters.UserId);
            if (expenseTypeExist)
            {
                return ExpenseTypeAlreadyExist;
            }

            var type = ExpenseType.Create(parameters.UserId, parameters.Name);
            await _context.ExpenseTypes.AddAsync(type);
            await _context.SaveChangesAsync();

            return new ExpenseTypeModel(type);
        }
    }

    public class CreateExpenseTypeParameters
    {
        public CreateExpenseTypeParameters(Guid userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public Guid UserId { get; }

        public string Name { get; }
    }
}
