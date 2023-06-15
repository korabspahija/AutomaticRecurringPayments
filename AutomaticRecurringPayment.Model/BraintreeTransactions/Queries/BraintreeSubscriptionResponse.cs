using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Queries
{
    public class BraintreeSubscriptionResponse
    {
        public Braintree.Subscription Subscription { get; set; }
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }
}
