using AutomaticRecurringPayments.Core.Configurations;
using Microsoft.Extensions.Configuration;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class ConfigurationDependencyInjectionExtensions
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            BindDatabaseConfiguration(services, configuration);
        }

        private static void BindDatabaseConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var databaseConfiguration = new DatabaseConfiguration();
            configuration.Bind("DbContextSettings", databaseConfiguration);
            services.AddSingleton(databaseConfiguration);
        }
    }
}
