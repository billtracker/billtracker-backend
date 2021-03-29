using System;
using BillTracker.Commands;
using BillTracker.Entities;
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

        internal Expense CreateExpense(Guid userId, string name = "expense")
        {
            var expenseType = CreateExpenseType(userId);
            var addExpense = Factory.Services.GetRequiredService<AddExpense>();
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var expense = addExpense.Handle(new AddExpenseParameters(userId, name, 20, expenseType.Id)).GetAwaiter().GetResult();
            return context.Expenses.Find(expense.Result.Id);
        }
    }
}
