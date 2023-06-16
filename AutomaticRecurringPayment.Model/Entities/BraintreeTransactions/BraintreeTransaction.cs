using AutomaticRecurringPayment.Model.Entities.Clients;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Entities.BraintreeTransactions
{
    public class BraintreeTransaction : BaseEntity
    {
        public string BraintreeCustomerId { get; set; }
        public string PaymentMethodToken { get; set; }
        public string TransactionId { get; set; }
        public long Amount { get; set; }
        public int? BraintreeTransactionStatusId { get; set; }
        public int ClientId { get; set; }
        public int? ParentBraintreeTransactionId { get; set; }
        public bool? RecurringCanceled { get; set; }
        public int? OnDemandStatus { get; set; }
        public int SubscriptionId { get; set; }
        public BraintreeTransaction ParentBraintreeTransaction { get; set; }
        public List<BraintreeTransaction> ChildBraintreeTransactions { get; set; }
        public Client Client { get; set; }
        public Subscription Subscription { get; set; }
        public Subscription LastSubscription { get; set; }
    }
}
