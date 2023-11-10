using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Commands
{
    public class CancelSubscriptionCommand : IRequest<CancelSubscriptionResponse>
    {
        public int SubscriptionId { get; set; }
        public int ClientId { get; set; }
    }
}
