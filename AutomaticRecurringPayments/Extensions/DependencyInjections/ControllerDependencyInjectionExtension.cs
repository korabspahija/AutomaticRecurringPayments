namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class ControllerDependencyInjectionExtension
    {
        public static void RegisterControllers(this IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(opt => {
                opt.SuppressModelStateInvalidFilter = true;

            }).AddFluentValidation();
        }
    }
}
