using FluentValidation.AspNetCore;
using System.Reflection;

namespace AutomaticRecurringPayments.Extensions.DependencyInjections
{
    public static class FluentValidationDependencyInjectionExtension
    {
        public static void AddFluentValidation(this IMvcBuilder mvc)
        {
            //mvc.AddFluentValidation(opt => {
            //    var assemblies = new Assembly[] {
            //        typeof(TestValidation).Assembly
            //    };
            //    opt.RegisterValidatorsFromAssemblies(assemblies);
            //    opt.ImplicitlyValidateChildProperties = true;
            //});

        }
    }
}
