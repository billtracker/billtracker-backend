using System;
using System.Threading.Tasks;
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
            var user = User.Create($"{Guid.NewGuid()}-xyz@syz.com", "pass", "Test", "Test");

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
    }
}
