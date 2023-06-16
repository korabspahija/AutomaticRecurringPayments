using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.JobServices
{
    public interface IBraintreeTransactionJobService
    {
        Task<IEnumerable<(BraintreeTransaction transaction, int subscriptionId, DateTime periodEnd)>> GetReadyToScheduleOnDemandBraintreeTransactions();
        Task ProcessScheduleRecurrentTransactions();
        Task<bool> ChargeBraintreeTransactionRecurrentSubscriptionAsync(int subscriptionId, int transactionId);
    }
}
