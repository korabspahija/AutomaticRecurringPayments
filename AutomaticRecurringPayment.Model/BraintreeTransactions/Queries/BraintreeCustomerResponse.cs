using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Queries
{
    public class BraintreeCustomerResponse
    {
        public Braintree.Customer Customer { get; set; }
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public ValidationErrors Errors { get; set; }
        public string Message { get; set; }
    }
}
