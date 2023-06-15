using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Entities.Clients
{
    public class Client : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ConsumerCode { get; set; }
        public string BraintreeCustomerId { get; set; }

        public List<Subscription> Subscriptions { get; set; }
        public List<BraintreeTransaction> BraintreeTransactions { get; set; }

    }
}
