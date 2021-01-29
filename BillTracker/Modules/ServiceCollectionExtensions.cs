using BillTracker.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BillTracker.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.ConfigureDatabase(configuration, isDevelopment);

            services.AddConfiguration<IdentityConfiguration>(configuration, IdentityConfiguration.SectionName);

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

        public static IServiceCollection AddConfiguration<TConfiguration>(
           this IServiceCollection services,
           IConfiguration configuration,
           string sectionName = null,
           bool optional = false)
           where TConfiguration : class, new()
        {
            if (sectionName == null)
            {
                sectionName = typeof(TConfiguration).Name;
            }

            var config = configuration.GetSection<TConfiguration>(sectionName, optional);

            services.TryAddSingleton(config);

            return services;
        }
    }
}
