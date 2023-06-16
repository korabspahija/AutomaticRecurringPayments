using AutomaticRecurringPayments.Core.JobServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Configuration.Extensions
{
    public static class JobServicesExtension
    {
        public static void RegisterJobServices(this IServiceCollection services)
        {
            services.AddScoped<IBraintreeTransactionJobService, BraintreeTransactionJobService>();
        }
    }
}
