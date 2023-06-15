using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyEntityTypes(this ModelBuilder builder, Assembly assembly, Type configInterface)
        {
            var typesToRegister = GetTypes(assembly, configInterface);

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                builder.ApplyConfiguration(configurationInstance);
            }
        }
        private static List<Type> GetTypes(Assembly assembly, Type configInterface)
        {
            var typesToRegister = assembly.GetTypes().Where(t => t.GetInterfaces()
            .Any(gi => gi.IsGenericType && gi.GetGenericTypeDefinition() == configInterface))
                .Where(x => !x.IsAbstract).ToList();

            return typesToRegister;
        }
    }
}
