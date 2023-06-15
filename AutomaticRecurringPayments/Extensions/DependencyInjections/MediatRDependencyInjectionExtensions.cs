using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayments.Core.Handlers.BraintreeTransactions;
using MediatR;
using System.Reflection;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class MediatRDependencyInjectionExtensions
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddTransient<IRequestHandler<CreateBraintreeTransactionCommand, CreateBraintreeTransactionResponse>, CreateBraintreeTransactionHandler>();

        }
    }
}
