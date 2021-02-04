using System;
using System.Linq;
using System.Threading.Tasks;
using BillTracker.Api;
using BillTracker.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace BillTracker.Tests
{
    public class BillTrackerWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("IntegrationTests")
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BillTrackerContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<BillTrackerContext>(opt =>
                    {
                        opt.UseInMemoryDatabase("InMemoryDbForTesting");
                    }, contextLifetime: ServiceLifetime.Transient);

                    using var context = services.BuildServiceProvider().GetRequiredService<BillTrackerContext>();
                    context.Database.EnsureCreated();
                });
        }
    }

    public class BillTrackerFixture : IClassFixture<BillTrackerWebApplicationFactory>
    {
        private static readonly BillTrackerWebApplicationFactory Factory = new BillTrackerWebApplicationFactory();

        internal BillTrackerWebApplicationFactory GetWebApplicationFactory() => Factory;

        internal async Task<User> CreateUser()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BillTrackerContext>();
            var user = User.Create($"{Guid.NewGuid()}-xyz@syz.com", "pass", "Test", "Test");

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
