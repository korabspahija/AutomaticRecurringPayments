using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Constants
{
    public static class SubscriptionStatusConstants
    {
        public const int Incomplete = 1;
        public const int Active = 2;
        public const int Expired = 3;
        public const int Canceled = 4;
        public const int PreCanceled = 5;
        public const int Scheduled = 6;
    }
}
