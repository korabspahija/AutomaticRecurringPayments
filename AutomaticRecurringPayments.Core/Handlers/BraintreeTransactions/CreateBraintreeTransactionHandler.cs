using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayment.Model.BraintreeTransactions.Queries;
using AutomaticRecurringPayment.Model.Constants;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using Braintree;
using MediatR;
using AutomaticRecurringPayments.Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace AutomaticRecurringPayments.Core.Handlers.BraintreeTransactions
{
    public class CreateBraintreeTransactionHandler : IRequestHandler<CreateBraintreeTransactionCommand, CreateBraintreeTransactionResponse>
    {
        private readonly IBraintreeTransactionService _braintreeTransactionService;
        private readonly IClientService _clientService;
        private readonly IBraintreeService _braintreeService;
        private readonly ISubscriptionService _subscriptionService;

        public CreateBraintreeTransactionHandler(
            IBraintreeTransactionService braintreeTransactionService,
            IClientService clientService,
            IBraintreeService braintreeService,
            ISubscriptionService subscriptionService
            )
        {
            _braintreeTransactionService = braintreeTransactionService;
            _clientService = clientService;
            _braintreeService = braintreeService;
            _subscriptionService = subscriptionService;
        }

        public async Task<CreateBraintreeTransactionResponse> Handle(
            CreateBraintreeTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var response = new CreateBraintreeTransactionResponse();

            try
            {
                var client = await _clientService.GetByIdAsync(request.ClientId, cancellationToken);
                if (client == null)
                    return null;

                var amount = await GetTotalAmount(client.ConsumerCode.ToString());
                if (amount <= 0)
                    return null;

                var transactionRequest = new TransactionRequest
                {
                    PaymentMethodNonce = request.Nonce,
                    DeviceData = request.DeviceData,
                    Amount = amount,
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true,
                        StoreInVaultOnSuccess = true
                    }
                };

                string paymentMethodToken = "";

                var braintreeCustomerResponse = new BraintreeCustomerResponse();
                braintreeCustomerResponse.Customer = await _braintreeService.GetCustomerAsync(client.BraintreeCustomerId);
                var foundBraintreeCustomer = braintreeCustomerResponse.Customer;
                var transactionId = "NotCreated" + Guid.NewGuid().ToString("N");
                var success = true;
                Transaction transaction = null;


                if (foundBraintreeCustomer == null)
                {
                    transactionRequest.Customer = new CustomerRequest
                    {
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Phone = client.PhoneNumber
                    };
                }
                else
                {
                    transactionRequest.CustomerId = foundBraintreeCustomer.Id;
                }

                var braintreeTransactionResponse = await _braintreeService.CreateTransactionAsync(transactionRequest);
                transaction = braintreeTransactionResponse?.Transaction;
                if (transaction == null || !(braintreeTransactionResponse?.Success ?? false))
                {
                    response.Success = false;
                    response.ErrorMessage = braintreeTransactionResponse?.Message;

                    if (foundBraintreeCustomer == null)
                    {
                        var customerRequest = new CustomerRequest
                        {
                            Email = client.Email,
                            FirstName = client.FirstName,
                            LastName = client.LastName,
                            Phone = client.PhoneNumber,
                            DeviceData = request.DeviceData,
                            PaymentMethodNonce = request.Nonce,
                            CreditCard = new CreditCardRequest
                            {
                                Options = new CreditCardOptionsRequest
                                {
                                    VerifyCard = true,
                                }
                            }
                        };

                        braintreeCustomerResponse = await _braintreeService.CreateCustomerAsync(customerRequest);
                        if (!(braintreeCustomerResponse?.Success ?? false))
                        {
                            response.ErrorMessage += " " + braintreeCustomerResponse?.Message;
                        }
                        else
                        {
                            foundBraintreeCustomer = braintreeCustomerResponse.Customer;
                            client.BraintreeCustomerId = foundBraintreeCustomer.Id;
                            client = _clientService.Update(client);
                            await _clientService.SaveChangesAsync();
                            paymentMethodToken = foundBraintreeCustomer.DefaultPaymentMethod?.Token ?? foundBraintreeCustomer.PaymentMethods?.FirstOrDefault()?.Token;
                        }
                    }
                }
                transactionId = transaction?.Id ?? transactionId;
                success = braintreeTransactionResponse?.Success ?? false;
                if (success)
                {
                    response.Success = true;
                    if (string.IsNullOrWhiteSpace(client.BraintreeCustomerId) && transaction?.CustomerDetails != null)
                    {
                        client.BraintreeCustomerId = transaction.CustomerDetails.Id;
                        client = _clientService.Update(client);
                        await _clientService.SaveChangesAsync();
                    }
                    paymentMethodToken = transaction.CreditCard?.Token;
                }

                AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription subscription = new AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription
                {
                    ClientId = client.Id,
                    StatusId = SubscriptionStatusConstants.Active,
                };

                subscription = await _subscriptionService.CreateAsync(subscription, cancellationToken);
                await _subscriptionService.SaveChangesAsync();
                

                var braintreeTransaction = new BraintreeTransaction
                {
                    ClientId = client.Id,
                    Amount = Convert.ToInt64((transaction?.Amount ?? 0) * 100),
                    PaymentMethodToken = string.IsNullOrWhiteSpace(paymentMethodToken) ? foundBraintreeCustomer?.DefaultPaymentMethod?.Token : paymentMethodToken,
                    TransactionId = transactionId,
                    BraintreeTransactionStatusId = TransactionStatusHelper.PrepareBraintreeTransactionStatus(transaction?.Status),
                    BraintreeCustomerId = transaction?.CustomerDetails?.Id ?? foundBraintreeCustomer?.Id,
                    OnDemandStatus = PaymentOnDemandStatusConstants.Ready,
                    SubscriptionId = subscription.Id
                };
                braintreeTransaction = await _braintreeTransactionService.CreateAsync(braintreeTransaction, cancellationToken);
                await _braintreeTransactionService.SaveChangesAsync();

                subscription.LastBraintreeTransactionId = braintreeTransaction.Id;
                subscription = _subscriptionService.Update(subscription);
                await _subscriptionService.SaveChangesAsync();

                // TODO: Add job to call webhook
                //await SchedulerJob.ScheduleUniqueJobAsync<IBraintreePaymentJob>(x => x.CallWebhookByTransactionId(null, braintreeTransaction.Id, application.PublicId, false, 0), TimeSpan.FromSeconds(0));

                
                return response;
            }

            catch (Exception ex)
            {
                return response;
                throw;
            }
        }

        private async Task<decimal> GetTotalAmount(string code)
        {
            IWebDriver driver = new OpenQA.Selenium.Edge.EdgeDriver();

            driver.Navigate().GoToUrl("http://kartela.kru-prishtina.com/Security/SearchCustomer?code=" + code);

            // Find the element with the desired HTML snippet
            IWebElement element = driver.FindElement(By.CssSelector("div.form-group p:nth-child(6) span"));

            // Get the inner text of the element, which represents the amount
            string amountText = element.Text;
            string amountWithoutSymbol = Regex.Replace(amountText, @"€", string.Empty);
            string amountWithoutComma = amountWithoutSymbol.Replace(",", ".");

            Thread.Sleep(2700);

            // Close the driver
            driver.Quit();

            var parsed = decimal.TryParse(amountWithoutComma, out decimal amountInDecimal);
            if (!parsed)
                return default;

            return amountInDecimal;
        }
    }
}