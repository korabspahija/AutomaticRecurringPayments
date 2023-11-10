using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Commands
{
    public class RefundBraintreeTransactionResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
