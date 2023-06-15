using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayment.Model.BraintreeTransactions.Queries;
using AutomaticRecurringPayment.Model.Constants;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.DatabaseContexts;
using Azure;
using Braintree;
using MediatR;
using AutomaticRecurringPayments.Core.Extensions;
using AutomaticRecurringPayments.Core.Helpers;

namespace AutomaticRecurringPayments.Core.Handlers.BraintreeTransactions
{
    public class CreateBraintreeTransactionHandler : IRequestHandler<CreateBraintreeTransactionCommand, CreateBraintreeTransactionResponse>
    {
        //private readonly IMapper _mapper;
        private readonly IBraintreeTransactionService _braintreeTransactionService;
        private readonly IClientService _clientService;
        private readonly IBraintreeService _braintreeService;
        private readonly ISubscriptionService _subscriptionService;

        public CreateBraintreeTransactionHandler(
            //IMapper mapper,
            IBraintreeTransactionService braintreeTransactionService,
            IClientService clientService,
            IBraintreeService braintreeService,
            ISubscriptionService subscriptionService
            )
        {
            //_mapper = mapper;
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

                var transactionRequest = new TransactionRequest
                {
                    PaymentMethodNonce = request.Nonce,
                    DeviceData = request.DeviceData,
                    Amount = 10,
                    //TransactionSource = "recurring_first",  // TODO: Check this
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true,
                        StoreInVaultOnSuccess = true
                    }
                };

                string paymentMethodToken = "";
                var client = await _clientService.GetByIdAsync(request.ClientId, cancellationToken);
                if (client == null)
                    return null;

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
                    StatusId = SubscriptionStatusConstants.Incomplete,
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
    }
}