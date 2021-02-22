using System;
using System.Threading.Tasks;
using BillTracker.Api;
using BillTracker.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BillTracker.Tests
{
    public class BillTrackerWebApplicationFactory : WebApplicationFactory<Startup> { }

    public class BillTrackerFixture : IClassFixture<BillTrackerWebApplicationFactory>
    {
        private static readonly BillTrackerWebApplicationFactory Factory = new BillTrackerWebApplicationFactory();

        internal BillTrackerWebApplicationFactory GetWebApplicationFactory() => Factory;

        internal async Task<User> CreateUser()
        {
            var context = Factory.Services.GetRequiredService<BillTrackerContext>();
            var user = User.Create($"{Guid.NewGuid()}-xyz@syz.com", "pass", "Test", "Test");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
