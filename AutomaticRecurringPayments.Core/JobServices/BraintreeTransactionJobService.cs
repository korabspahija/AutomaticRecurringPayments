using AutomaticRecurringPayment.Model.Constants;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.Helpers;
using AutomaticRecurringPayments.Core.Interfaces;
using Braintree;
using Hangfire;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace AutomaticRecurringPayments.Core.JobServices
{
    public class BraintreeTransactionJobService : IBraintreeTransactionJobService
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IBraintreeTransactionService _braintreeTransactionService;
        private readonly IBraintreeService _braintreeService;
        private readonly IClientService _clientService;

        public BraintreeTransactionJobService(
            ISubscriptionService subscriptionService,
            IBraintreeTransactionService braintreeTransactionService,
            IBraintreeService braintreeService,
            IClientService clientService
            )
        {
            _subscriptionService = subscriptionService;
            _braintreeTransactionService = braintreeTransactionService;
            _braintreeService = braintreeService;
            _clientService = clientService;
        }


        public async Task<IEnumerable<(BraintreeTransaction transaction, int subscriptionId, DateTime periodEnd)>> GetReadyToScheduleOnDemandBraintreeTransactions()
        {
            var subscriptions = await _subscriptionService.GetBraintreePaymentActiveSubscriptions();

            var braintreePaymentsToCharge = new List<(BraintreeTransaction, int, DateTime)>();
            foreach (var subscription in subscriptions)
            {
                if (subscription.LastBraintreeTransactionId.HasValue)
                {
                    var braintreeTransaction = await _braintreeTransactionService.GetByIdAsync(subscription.LastBraintreeTransactionId.Value);
                    if (braintreeTransaction == null)
                        continue;

                    if (!braintreeTransaction.OnDemandStatus.HasValue || braintreeTransaction.OnDemandStatus.Value != PaymentOnDemandStatusConstants.Ready)
                        continue;

                    braintreePaymentsToCharge.Add((braintreeTransaction, subscription.Id, braintreeTransaction.InsertDateTime.Value.AddMonths(1)));
                }
            }
            return braintreePaymentsToCharge;
        }

        public async Task ProcessScheduleRecurrentTransactions()
        {
            var readyToScheduleBraintreeTransactions = await GetReadyToScheduleOnDemandBraintreeTransactions();

            foreach (var braintreeTransaction in readyToScheduleBraintreeTransactions)
            {
                if (braintreeTransaction.Item1.OnDemandStatus != PaymentOnDemandStatusConstants.Ready)
                    continue;

                new BackgroundJobClient().Schedule<IBraintreeTransactionJob>(x => x.CreateTransaction(braintreeTransaction.subscriptionId, braintreeTransaction.transaction.Id, null),
                    braintreeTransaction.periodEnd > DateTime.UtcNow ? braintreeTransaction.periodEnd - DateTime.UtcNow : TimeSpan.FromSeconds(1));

                braintreeTransaction.Item1.OnDemandStatus = PaymentOnDemandStatusConstants.RequestScheduled;
                _braintreeTransactionService.Update(braintreeTransaction.Item1);
                await _braintreeTransactionService.SaveChangesAsync();
            }
        }

        public async Task<bool> ChargeBraintreeTransactionRecurrentSubscriptionAsync(int subscriptionId, int transactionId)
        {
            try
            {
                AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription subscription = await _subscriptionService.GetByIdAsync(subscriptionId);
                if (subscription == null)
                    throw new Exception($"Subscription is null. transactionId: {transactionId}");

                if (subscription.StatusId != SubscriptionStatusConstants.Active)
                    throw new Exception($"Subscription is not active. transactionId: {transactionId}");

                if (!subscription.LastBraintreeTransactionId.HasValue)
                    throw new Exception($"Subscription has no transaction: {subscription.Id}");

                var client = await _clientService.GetByIdAsync(subscription.ClientId);
                if (client == null)
                    throw new Exception($"Client does not exist: {subscription.ClientId}");

                var amount = await GetTotalAmount(client.ConsumerCode.ToString());
                if (amount <= 0)
                    throw new Exception($"Consumer total amount is: {amount}");

                var lastBraintreeTransaction = await _braintreeTransactionService.GetByIdAsync(subscription.LastBraintreeTransactionId.Value);
                if (lastBraintreeTransaction == null)
                {
                    throw new Exception("lastBraintreeTransaction is null");
                }

                var periodEnd = lastBraintreeTransaction.InsertDateTime.Value.AddMonths(1);
                if (periodEnd > DateTime.Now.AddHours(3))
                {
                    if (subscription.LastBraintreeTransactionId == transactionId)
                    {
                        new BackgroundJobClient().Schedule<IBraintreeTransactionJob>(x => x.CreateTransaction(subscriptionId, transactionId, null),
                            lastBraintreeTransaction.InsertDateTime.Value.AddMonths(1) > DateTime.UtcNow ? lastBraintreeTransaction.InsertDateTime.Value.AddMonths(1) - DateTime.UtcNow : TimeSpan.FromSeconds(1));
                        return false;
                    }
                    throw new Exception($"Old payment is repeated, transactionId: {transactionId}, Subscription.LastInvoiceId: {subscription.LastBraintreeTransactionId}, transactionId: {transactionId}");
                }


                if (lastBraintreeTransaction.OnDemandStatus != PaymentOnDemandStatusConstants.RequestScheduled)
                    throw new Exception("Braintree onDemandStatus is " + lastBraintreeTransaction.OnDemandStatus);


                BraintreeTransaction parentBraintreeTransaction = null;

                if (lastBraintreeTransaction.ParentBraintreeTransactionId.HasValue)
                    parentBraintreeTransaction = await _braintreeTransactionService.GetByIdAsync(lastBraintreeTransaction.ParentBraintreeTransactionId.Value);
                else
                    parentBraintreeTransaction = lastBraintreeTransaction;

                if (parentBraintreeTransaction.RecurringCanceled.HasValue && parentBraintreeTransaction.RecurringCanceled.Value)
                {
                    lastBraintreeTransaction.OnDemandStatus = PaymentOnDemandStatusConstants.RequestFailed;
                    _braintreeTransactionService.Update(lastBraintreeTransaction);
                    await _braintreeTransactionService.SaveChangesAsync();
                    return false;
                }


                var newTransactionRequest = new TransactionRequest
                {
                    PaymentMethodToken = lastBraintreeTransaction.PaymentMethodToken,
                    //TransactionSource = "unscheduled", // TODO: check this
                    //Options = new TransactionOptionsRequest
                    //{
                    //    SubmitForSettlement = true,
                    //},
                    Amount = amount
                };

                var newlyCreatedTransactionId = "NotCreated" + Guid.NewGuid().ToString("N");
                var braintreeTransactionResponse = await _braintreeService.CreateTransactionAsync(newTransactionRequest);
                var newTransaction = braintreeTransactionResponse?.Transaction;
                if (newTransaction == null || !(braintreeTransactionResponse?.Success ?? false))
                {
                    lastBraintreeTransaction.OnDemandStatus = PaymentOnDemandStatusConstants.RequestFailed;
                    _braintreeTransactionService.Update(lastBraintreeTransaction);
                    await _braintreeTransactionService.SaveChangesAsync();
                }
                else
                {
                    lastBraintreeTransaction.OnDemandStatus = PaymentOnDemandStatusConstants.RequestCompleted;
                    _braintreeTransactionService.Update(lastBraintreeTransaction);
                    await _braintreeTransactionService.SaveChangesAsync();
                }
                newlyCreatedTransactionId = newTransaction?.Id ?? newlyCreatedTransactionId;


                var nextBraintreeTransaction = new BraintreeTransaction
                {
                    ClientId = lastBraintreeTransaction.ClientId,
                    PaymentMethodToken = lastBraintreeTransaction.PaymentMethodToken,
                    TransactionId = newlyCreatedTransactionId,
                    Amount = Convert.ToInt64(amount * 100),
                    BraintreeTransactionStatusId = TransactionStatusHelper.PrepareBraintreeTransactionStatus(newTransaction?.Status),
                    OnDemandStatus = PaymentOnDemandStatusConstants.Ready,
                    BraintreeCustomerId = lastBraintreeTransaction.BraintreeCustomerId,
                    ParentBraintreeTransactionId = parentBraintreeTransaction.Id,
                    SubscriptionId = subscription.Id
                };

                nextBraintreeTransaction = await _braintreeTransactionService.CreateAsync(nextBraintreeTransaction);
                await _braintreeTransactionService.SaveChangesAsync();

                subscription.LastBraintreeTransactionId = nextBraintreeTransaction.Id;
                _subscriptionService.Update(subscription);
                await _subscriptionService.SaveChangesAsync();

                //send webhook events

                return braintreeTransactionResponse?.Success ?? false;
            }
            catch (Exception ex)
            {
                throw new Exception($"IBraintreePaymentJob.CreateRecurrentTransaction failed with exception : {ex.Message}");
            }
        }

        private async Task<decimal> GetTotalAmount(string code)
        {
            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl("http://kartela.kru-prishtina.com/Security/SearchCustomer?code=" + code);

            // Find the element with the desired HTML snippet
            IWebElement element = driver.FindElement(By.CssSelector("div.form-group p:nth-child(6) span"));

            // Get the inner text of the element, which represents the amount
            string amountText = element.Text;
            string amountWithoutSymbol = Regex.Replace(amountText, @"€", string.Empty);
            string amountWithoutComma = amountWithoutSymbol.Replace(",", ".");
            // Close the driver
            driver.Quit();

            var parsed = decimal.TryParse(amountWithoutComma, out decimal amountInDecimal);
            if (!parsed)
                return default;

            return amountInDecimal;
        }
    }
}
