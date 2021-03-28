using BillTracker.Commands;
using BillTracker.Entities;
using BillTracker.Identity;
using BillTracker.Queries;
using BillTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BillTracker.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureDatabase(configuration);

            services.AddConfiguration<IdentityConfiguration>(configuration, IdentityConfiguration.SectionName);
            services.AddConfiguration<AzureConfiguration>(configuration, AzureConfiguration.SectionName);

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBillFileStorage, AzureBillBlobStorage>();

            // Commands
            services.AddTransient<AddExpense>();
            services.AddTransient<AddExpenseBillFile>();
            services.AddTransient<CreateExpenseType>();
            services.AddTransient<SetupNewUser>();
            services.AddTransient<SaveExpenseAggregate>();

            // Queries
            services.AddTransient<ExpensesQuery>();
            services.AddTransient<ExpenseTypesQuery>();
            services.AddTransient<DashboardQuery>();

            if (configuration.GetValue<bool>("InitializeOnStartup"))
            {
                var serviceProvider = services.BuildServiceProvider();
                serviceProvider.GetService<BillTrackerContext>().Database.Migrate();

                serviceProvider.InitializeServices(services);
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

        private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
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

            return services;
        }

        private static void InitializeServices(this ServiceProvider serviceProvider, IServiceCollection services)
        {
            // Services to initialize
            services.AddInitializable<AzureBillBlobStorage>();

            // Initialize those services
            var servicesToInitialize = serviceProvider.GetServices<IInitializable>();
            foreach (var service in servicesToInitialize)
            {
                service.Initialize();
            }

            services.RemoveAll<IInitializable>();
        }

        private static void AddInitializable<TService>(this IServiceCollection services)
            where TService : class, IInitializable
        {
            services.AddTransient<IInitializable, TService>();
        }
    }
}
