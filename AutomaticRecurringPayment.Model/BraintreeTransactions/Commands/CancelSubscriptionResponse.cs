using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Commands
{
    public class CancelSubscriptionResponse
    {
        public bool Canceled { get; set; }
    }
}
