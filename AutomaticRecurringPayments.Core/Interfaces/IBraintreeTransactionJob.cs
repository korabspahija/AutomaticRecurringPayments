using Hangfire.Server;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Interfaces
{
    public interface IBraintreeTransactionJob
    {
        [Queue("schedule_recurrent_transaction")]
        Task<bool> ScheduleRecurrentTransaction(PerformContext performContext);

        [Queue("create_transaction")]
        Task<bool> CreateTransaction(int subscriptionId, int transactionId, PerformContext context);
    }
}
