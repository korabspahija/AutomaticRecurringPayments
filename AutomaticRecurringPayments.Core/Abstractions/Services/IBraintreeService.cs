using AutomaticRecurringPayment.Model.BraintreeTransactions.Queries;
using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Abstractions.Services
{
    public interface IBraintreeService
    {
        Task<BraintreeTransactionResponse> CreateTransactionAsync(TransactionRequest transactionRequest);
        Task<Transaction> GetTransactionAsync(string transactionId);
        Task<string> GenerateClientToken(string customerId = null);
        Task<BraintreeSubscriptionResponse> CreateSubscriptionAsync(SubscriptionRequest subscriptionRequest);
        Task<Braintree.CreditCard> GetCreditCardFromNonceAsync(string nonce);
        Task<Braintree.CreditCard> CreatePaymentMethodAsync(PaymentMethodRequest paymentMethodRequest);
        Task<Braintree.CreditCard> UpdatePaymentMethodAsync(string token, PaymentMethodRequest paymentMethodRequest);
        Task<BraintreeCustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest);
        Task<BraintreeCustomerResponse> UpdateCustomerAsync(CustomerRequest customerRequest, string customerId);
        Task<Customer> GetCustomerAsync(string id);
        Task<BraintreeSubscriptionResponse> CancelSubscription(string id);
        Task<BraintreeTransactionResponse> RefundTransactionAsync(string transactionId, decimal amount);
        Task<BraintreeTransactionResponse> VoidTransactionAsync(string transactionId);
        Task<BraintreeSubscriptionResponse> GetSubscriptionAsync(string subscriptionId);
        Task<Plan> CreatePlanAsync(PlanRequest planRequest);
        Task<string> CreatePaymentMethodNonceAsync(string token);
        Task<PaymentMethod> DeletePaymentMethodAsync(string token);
    }
}
