using AutomaticRecurringPayments.Core.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using System.Diagnostics.CodeAnalysis;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class HangfireExtension
    {
        public static void RegisterHangfire(this IServiceCollection services)
        {
            services.AddHangfire(config =>
                config.UseStorage(HangfireSetting.GetDefaultBackgroundJobStorage())
            );
        }

        public static void UseHangfireDefaultDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorizeFilter() },
                IgnoreAntiforgeryToken = true
            }, HangfireSetting.GetDefaultBackgroundJobStorage());
        }
    }

    public class HangfireAuthorizeFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
