using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Identity;
using BillTracker.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BillTracker.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration, string environment)
        {
            services.ConfigureDatabase(configuration, environment);

            services.AddConfiguration<IdentityConfiguration>(configuration, IdentityConfiguration.SectionName);

            services.AddTransient<IIdentityService, IdentityService>();

            // Commands
            services.AddTransient<AddExpense>();
            services.AddTransient<CreateExpenseType>();

            // Queries
            services.AddTransient<ExpensesQuery>();
            services.AddTransient<DashboardQuery>();

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

        private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration, string environment)
        {
            services
                .AddDbContext<BillTrackerContext>(
                    opt =>
                    {
                        opt.UseNpgsql(
                            configuration.GetConnectionString("Database"),
                            builder => builder.EnableRetryOnFailure());
                    },
                    ServiceLifetime.Transient,
                    ServiceLifetime.Transient);

            if (environment != "IntegrationTests" && configuration.GetValue<bool>("MigrateDbOnStartup"))
            {
                services.BuildServiceProvider().GetService<BillTrackerContext>().Database.Migrate();
            }

            return services;
        }
    }
}
