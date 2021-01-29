using BillTracker.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillTracker.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.ConfigureDatabase(configuration, isDevelopment);

            services.AddTransient<IIdentityService, IdentityService>();

            return services;
        }

        private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services
                .AddDbContext<BillTrackerContext>(opt =>
                {
                    opt.UseNpgsql(
                        configuration.GetConnectionString("Database"),
                        builder => builder.EnableRetryOnFailure());
                }, ServiceLifetime.Transient);

            if (isDevelopment)
            {
                services.BuildServiceProvider().GetService<BillTrackerContext>().Database.Migrate();
            }

            return services;
        }
    }
}
