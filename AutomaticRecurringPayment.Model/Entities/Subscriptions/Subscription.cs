using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Entities.Subscriptions
{
    public class Subscription : BaseEntity
    {
        public int ClientId { get; set; }
        public int StatusId { get; set; }
        public int? LastBraintreeTransactionId { get; set; }

        public BraintreeTransaction LastBraintreeTransaction { get; set; }
        public List<BraintreeTransaction> BraintreeTransactions { get; set; }
        public Client Client { get; set; }
    }
}
