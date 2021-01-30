using System.Linq;
using BillTracker.Api;
using BillTracker.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
}
