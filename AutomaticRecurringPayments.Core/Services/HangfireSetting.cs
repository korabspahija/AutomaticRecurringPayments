using Hangfire.SqlServer;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Services
{
    public class HangfireSetting
    {
        private static JobStorage experimentJobStorage = null;
        public static JobStorage GetDefaultBackgroundJobStorage()
        {
            var connectionString = "Server=KORAB\\SQLEXPRESS;Database=Automatic.Recurring.Payments;MultipleActiveResultSets=true;Integrated Security=True;TrustServerCertificate=True";
            return new SqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                SchemaName = "hangfire.default"
            });
        }
    }
}
