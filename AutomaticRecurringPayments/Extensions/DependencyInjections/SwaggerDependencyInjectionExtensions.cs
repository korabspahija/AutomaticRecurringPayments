using Microsoft.OpenApi.Models;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class SwaggerDependencyInjectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Automatic recurring payments", Version = "v1" });
            });
        }

        public static void UseSwaggerDI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "Automatic recurring payments v1");
                options.DocumentTitle = "Automatic.Recurring.Payments";
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
            });
        }
    }
}
