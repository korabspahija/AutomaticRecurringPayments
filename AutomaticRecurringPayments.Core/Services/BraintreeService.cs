using AutomaticRecurringPayment.Model.BraintreeTransactions.Queries;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using Braintree;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Services
{
    public class BraintreeService : IBraintreeService
    {
        private ILogger<BraintreeService> _logger;

        // TODO: Add a static BraintreeGateway and use that one after connecting instead of connecting for every request.

        public BraintreeService(ILogger<BraintreeService> logger)
        {
            _logger = logger;
        }
        public async Task<BraintreeGateway> Connect()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "pf8gj4dzz3hnvfpd",
                PublicKey = "8fgtmwwhz2fbvqdr",
                PrivateKey = "4a978de606bd2254388502725f91288d"
            };

            return gateway;
        }
        public async Task<string> GenerateClientToken(string customerId = null)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                Customer customer = null;
                if (!string.IsNullOrEmpty(customerId))
                    customer = await GetCustomerAsync(customerId);

                string clientToken = null;
                if (customer == null)
                {
                    clientToken = await gateway.ClientToken.GenerateAsync(new ClientTokenRequest());
                }
                else
                {
                    clientToken = await gateway.ClientToken.GenerateAsync(new ClientTokenRequest
                    {
                        CustomerId = customer.Id,
                    });
                }

                return clientToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<BraintreeTransactionResponse> RefundTransactionAsync(string transactionId, decimal amount)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var refundResult = await gateway.Transaction.RefundAsync(transactionId, amount);
                if (refundResult.IsSuccess())
                {
                    return new BraintreeTransactionResponse
                    {
                        Transaction = refundResult.Target,
                        Exception = null,
                        Success = true
                    };
                }

                return new BraintreeTransactionResponse
                {
                    Transaction = refundResult.Target,
                    Exception = null,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                return new BraintreeTransactionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<BraintreeTransactionResponse> VoidTransactionAsync(string transactionId)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var voidResult = await gateway.Transaction.VoidAsync(transactionId);
                if (voidResult.IsSuccess())
                {
                    return new BraintreeTransactionResponse
                    {
                        Transaction = voidResult.Target,
                        Exception = null,
                        Success = true
                    };
                }

                return new BraintreeTransactionResponse
                {
                    Transaction = voidResult.Target,
                    Exception = null,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                return new BraintreeTransactionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<BraintreeTransactionResponse> CreateTransactionAsync(TransactionRequest transactionRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var saleResult = await gateway.Transaction.SaleAsync(transactionRequest);
                //check if customer exist, if not create it
                if (saleResult.IsSuccess())
                {
                    if (transactionRequest.Options == null)
                    {
                        Result<Transaction> result = gateway.Transaction.SubmitForSettlement(saleResult.Target.Id);
                        if (result.IsSuccess())
                        {
                            return new BraintreeTransactionResponse
                            {
                                Transaction = result.Target ?? saleResult.Transaction,
                                Exception = null,
                                Errors = result.Errors,
                                Message = result.Message,
                                Success = true
                            };
                        }
                        return new BraintreeTransactionResponse
                        {
                            Transaction = result.Target ?? saleResult.Transaction,
                            Exception = null,
                            Errors = result.Errors,
                            Message = result.Message,
                            Success = true
                        };
                    }
                    return new BraintreeTransactionResponse
                    {
                        Transaction = saleResult.Target ?? saleResult.Transaction,
                        Exception = null,
                        Errors = saleResult.Errors,
                        Message = saleResult.Message,
                        Success = true
                    };
                }

                return new BraintreeTransactionResponse
                {
                    Transaction = saleResult.Target ?? saleResult.Transaction,
                    Exception = null,
                    Errors = saleResult.Errors,
                    Message = saleResult.Message,
                    Success = false
                };
            }
            catch (Braintree.Exceptions.AuthorizationException ex)
            {
                return new BraintreeTransactionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<Transaction> GetTransactionAsync(string transactionId)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var transaction = await gateway.Transaction.FindAsync(transactionId);
                return transaction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<BraintreeSubscriptionResponse> GetSubscriptionAsync(string subscriptionId)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var subscripton = await gateway.Subscription.FindAsync(subscriptionId);

                return new BraintreeSubscriptionResponse
                {
                    Subscription = subscripton,
                    Exception = null,
                    Success = subscripton != null
                };
            }
            catch (Exception ex)
            {
                return new BraintreeSubscriptionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<BraintreeSubscriptionResponse> CancelSubscription(string id)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var subscriptionResult = await gateway.Subscription.CancelAsync(id);
                if (subscriptionResult.IsSuccess())
                {
                    return new BraintreeSubscriptionResponse
                    {
                        Subscription = subscriptionResult.Target,
                        Exception = null,
                        Success = true
                    };
                }

                return new BraintreeSubscriptionResponse
                {
                    Exception = new Exception(subscriptionResult.Message),
                    Success = false
                };
            }
            catch (Exception ex)
            {
                return new BraintreeSubscriptionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<BraintreeSubscriptionResponse> CreateSubscriptionAsync(SubscriptionRequest subscriptionRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var subscriptionResult = await gateway.Subscription.CreateAsync(subscriptionRequest);
                if (subscriptionResult.IsSuccess())
                {
                    return new BraintreeSubscriptionResponse
                    {
                        Subscription = subscriptionResult.Target,
                        Exception = null,
                        Success = true
                    };
                }

                return new BraintreeSubscriptionResponse
                {
                    Subscription = subscriptionResult.Target,
                    Exception = new Exception(subscriptionResult.Message),
                    Success = false
                };
            }
            catch (Exception ex)
            {
                return new BraintreeSubscriptionResponse
                {
                    Exception = ex,
                    Success = false
                };
            }
        }
        public async Task<Subscription> UpdateSubscriptionAsync(SubscriptionRequest subscriptionRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var subscriptionResult = await gateway.Subscription.UpdateAsync("", subscriptionRequest);
                if (subscriptionResult.IsSuccess())
                    return subscriptionResult.Target;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Plan> CreatePlanAsync(PlanRequest planRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var planResult = await gateway.Plan.CreateAsync(planRequest);
                if (planResult.IsSuccess())
                    return planResult.Target;

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Braintree.CreditCard> CreatePaymentMethodAsync(PaymentMethodRequest paymentMethodRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var paymentMethodResult = await gateway.PaymentMethod.CreateAsync(paymentMethodRequest);
                if (paymentMethodResult.IsSuccess())
                    return (Braintree.CreditCard)paymentMethodResult.Target;

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Braintree.CreditCard> UpdatePaymentMethodAsync(string token, PaymentMethodRequest paymentMethodRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var paymentMethodResult = await gateway.PaymentMethod.UpdateAsync(token, paymentMethodRequest);
                if (paymentMethodResult.IsSuccess())
                    return (Braintree.CreditCard)paymentMethodResult.Target;

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Braintree.CreditCard> GetCreditCardFromNonceAsync(string nonce)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var creditCard = await gateway.CreditCard.FromNonceAsync(nonce);
                return creditCard;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<BraintreeCustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var customerResult = await gateway.Customer.CreateAsync(customerRequest);
                if (customerResult.IsSuccess())
                {
                    return new BraintreeCustomerResponse
                    {
                        Customer = customerResult.Target,
                        Exception = null,
                        Errors = customerResult.Errors,
                        Message = customerResult.Message,
                        Success = true
                    };
                }

                return new BraintreeCustomerResponse
                {
                    Customer = customerResult.Target,
                    Exception = null,
                    Errors = customerResult.Errors,
                    Message = customerResult.Message,
                    Success = true
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<BraintreeCustomerResponse> UpdateCustomerAsync(CustomerRequest customerRequest, string customerId)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                Result<Customer> customerResult = await gateway.Customer.UpdateAsync(customerId, customerRequest);
                if (customerResult.IsSuccess())
                {
                    return new BraintreeCustomerResponse
                    {
                        Customer = customerResult.Target,
                        Exception = null,
                        Errors = customerResult.Errors,
                        Message = customerResult.Message,
                        Success = true
                    };
                }

                return new BraintreeCustomerResponse
                {
                    Customer = customerResult.Target,
                    Exception = null,
                    Errors = customerResult.Errors,
                    Message = customerResult.Message,
                    Success = true
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Customer> GetCustomerAsync(string id)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var customer = await gateway.Customer.FindAsync(id);
                return customer;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> CreatePaymentMethodNonceAsync(string token)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                Result<PaymentMethodNonce> result = await gateway.PaymentMethodNonce.CreateAsync(token);
                return result.Target.Nonce;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PaymentMethod> DeletePaymentMethodAsync(string token)
        {
            var gateway = await Connect();
            if (gateway == null) return null;
            try
            {
                var paymentMethodResult = await gateway.PaymentMethod.DeleteAsync(token);
                if (paymentMethodResult.IsSuccess())
                    return paymentMethodResult.Target;

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
