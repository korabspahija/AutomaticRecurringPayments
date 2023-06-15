using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Constants
{
    public static class PaymentOnDemandStatusConstants
    {
        public const int Ready = 1;
        public const int RequestScheduled = 2;
        public const int RequestCompleted = 3;
        public const int RequestFailed = 4;
    }
}
