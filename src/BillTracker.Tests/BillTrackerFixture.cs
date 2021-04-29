using System;
using System.Linq;
using BillTracker.Commands;
using BillTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests
{
    public class BillTrackerFixture : IClassFixture<BillTrackerWebApplicationFactory>
    {
        private static readonly BillTrackerWebApplicationFactory Factory = new BillTrackerWebApplicationFactory();

        internal BillTrackerWebApplicationFactory GetWebApplicationFactory() => Factory;

        internal User CreateUser()
        {
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var id = Guid.NewGuid();
            var user = User.Create($"email-{id}@syz.com", "pass", $"username-{id}");

            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }

        internal ExpenseType CreateExpenseType(Guid userId, string nameSuffix = "")
        {
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var expenseType = ExpenseType.Create(userId, $"test-expense-type{nameSuffix}");

            context.ExpenseTypes.Add(expenseType);
            context.SaveChanges();

            return expenseType;
        }

        internal ExpensesAggregate CreateExpenseAggregate(Guid userId, string name = "expense", DateTimeOffset? addedDate = null, bool isDraft = false)
        {
            addedDate ??= DateTimeOffset.Now;
            var saveAggregate = Factory.Services.GetRequiredService<SaveExpenseAggregate>();
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var aggId = saveAggregate.Handle(new SaveExpenseAggregateParameters(null, userId, name, price: 20, addedDate: addedDate, isDraft: isDraft)).GetAwaiter().GetResult();
            return context.ExpensesAggregates.AsNoTracking().Single(x => x.Id == aggId.Result);
        }

        internal Expense CreateExpense(Guid userId, string name = "expense", int amount = 1)
        {
            var aggId = CreateExpenseAggregate(userId).Id;
            var expenseType = CreateExpenseType(userId);
            var addExpense = Factory.Services.GetRequiredService<SaveExpense>();
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var expense = addExpense.Handle(new SaveExpenseParameters(userId, aggId, name, price: 20, amount: amount, expenseTypeId: expenseType.Id)).GetAwaiter().GetResult();
            return context.Expenses.AsNoTracking().Single(x => x.Id == expense.Result.Id);
        }
    }
}
