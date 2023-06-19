using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.Interfaces;
using AutomaticRecurringPayments.Core.JobServices;
using AutomaticRecurringPayments.Core.Services;
using Hangfire.Server;

namespace AutomaticRecurringPayments.Worker.Jobs
{
    public class BraintreeTransactionJob : IBraintreeTransactionJob
    {
        private readonly IBraintreeTransactionJobService _braintreeTransactionJobService;

        public BraintreeTransactionJob(IBraintreeTransactionJobService braintreeTransactionJobService)
        {
            _braintreeTransactionJobService = braintreeTransactionJobService;
        }

        public async Task<bool> ScheduleRecurrentTransaction(PerformContext performContext)
        {
            await _braintreeTransactionJobService.ProcessScheduleRecurrentTransactions();

            return true;
        }

        public async Task<bool> CreateTransaction(int subscriptionId, int transactionId, PerformContext context)
        {
            var result = await _braintreeTransactionJobService.ChargeBraintreeTransactionRecurrentSubscriptionAsync(subscriptionId, transactionId);
            return result;
        }
    }
}