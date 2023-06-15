
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.Services;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class ServicesDependencyInjectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IBraintreeTransactionService, BraintreeTransactionService>();
            services.AddScoped<IBraintreeService, BraintreeService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
        }
    }
}
