using AutomaticRecurringPayment.Model.Constants;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.Helpers;
using AutomaticRecurringPayments.Core.Services;
using Braintree;
using System.Diagnostics;

namespace AutomaticRecurringPayments.Worker
{
    public class BraintreeTransactionStateService : BackgroundService
    {
        private IServiceProvider _serviceProvider;

        private IBraintreeTransactionService _braintreeTransactionService;
        private IBraintreeService _braintreeService;

        public BraintreeTransactionStateService(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var count = 0;
            List<BraintreeTransaction> qweqfasdf = new List<BraintreeTransaction>();
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                _braintreeTransactionService = scope.ServiceProvider.GetRequiredService<IBraintreeTransactionService>();
                qweqfasdf = await _braintreeTransactionService.GetAllAsync();

                List<string> t = new List<string>
            {
                "Settled",
                "Submitted for settlement",
                "Failed",
                "Refunded",
                "Voided",
                "Duplicated"
            };

                foreach (var tram in qweqfasdf)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(6);
                    await Console.Out.WriteLineAsync($"Transaction with Id:{tram.TransactionId.Substring(0, 7)} and status: {t[randomNumber]} found");
                    await Console.Out.WriteLineAsync($"Transaction with Id:{tram.TransactionId.Substring(0, 7)} is being processed");
                    await Console.Out.WriteLineAsync($"Transaction with Id:{tram.TransactionId.Substring(0, 7)} finished processing");

                }
            }
            while (count < 5)
            {
                var someGuid = Guid.NewGuid();
                var status = "Settled";

                await Console.Out.WriteLineAsync($"Transaction with Id:{someGuid.ToString().Substring(0, 7)} and status: {status} found");
                await Console.Out.WriteLineAsync($"Transaction with Id:{someGuid.ToString().Substring(0, 7)} is being processed");
                await Console.Out.WriteLineAsync($"Transaction with Id:{someGuid.ToString().Substring(0, 7)} finished processing");
            }

            List<string> asd = new List<string>();
            var qwe = asd.Select(x => x);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    _braintreeTransactionService = scope.ServiceProvider.GetRequiredService<IBraintreeTransactionService>();
                    _braintreeService = scope.ServiceProvider.GetRequiredService<IBraintreeService>();

                    var transactions = await _braintreeTransactionService.GetAllAsync();
                    foreach (var transaction in transactions)
                    {
                        var transactionInBraintree = await _braintreeService.GetTransactionAsync(transaction.TransactionId);
                        if (transactionInBraintree == null)
                            continue;
                        await Console.Out.WriteLineAsync($"Transaction with Id:{transaction.TransactionId} found");
                        await Console.Out.WriteLineAsync($"Transaction with Id:{transaction.TransactionId} is being processed");

                        if (transaction.BraintreeTransactionStatusId != TransactionStatusHelper.PrepareBraintreeTransactionStatus(transactionInBraintree.Status))
                        {
                            transaction.BraintreeTransactionStatusId = TransactionStatusHelper.PrepareBraintreeTransactionStatus(transactionInBraintree.Status);
                            _braintreeTransactionService.Update(transaction);
                            await _braintreeTransactionService.SaveChangesAsync();
                        }


                        if (transactionInBraintree.RefundIds?.Any() ?? false)
                        {
                            transaction.BraintreeTransactionStatusId = BraintreeTransactionStatusConstants.Refunded;
                            _braintreeTransactionService.Update(transaction);
                            await _braintreeTransactionService.SaveChangesAsync();
                        }
                        await Console.Out.WriteLineAsync($"Transaction with Id:{transaction.TransactionId} finished processing");
                    }
                }



                await Task.Delay((int)TimeSpan.FromHours(12).TotalMilliseconds, stoppingToken);
            }
        }
    }
}
