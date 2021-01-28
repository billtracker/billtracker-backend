using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillTracker.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<BillTrackerContext>(opt =>
                {
                    opt.UseNpgsql(
                        configuration.GetConnectionString("Database"),
                        builder => builder.EnableRetryOnFailure());
                });

            services.BuildServiceProvider().GetService<BillTrackerContext>().Database.Migrate();

            return services;
        }
    }
}
