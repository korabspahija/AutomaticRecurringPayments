using AutoMapper;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class MapperDependencyInjectionExtension
    {
        public static void AddMappingWithProfiles(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                //cfg.AddProfile<TestMappings>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
