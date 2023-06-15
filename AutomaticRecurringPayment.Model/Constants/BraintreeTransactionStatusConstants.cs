using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.Constants
{
    public static class BraintreeTransactionStatusConstants
    {
        public const int AUTHORIZATION_EXPIRED = 1;
        public const int AUTHORIZED = 2;
        public const int AUTHORIZING = 3;
        public const int FAILED = 4;
        public const int GATEWAY_REJECTED = 5;
        public const int PROCESSOR_DECLINED = 6;
        public const int SETTLED = 7;
        public const int SETTLING = 8;
        public const int SUBMITTED_FOR_SETTLEMENT = 9;
        public const int VOIDED = 10;
        public const int UNRECOGNIZED = 11;
        public const int SETTLEMENT_CONFIRMED = 12;
        public const int SETTLEMENT_DECLINED = 13;
        public const int SETTLEMENT_PENDING = 14;



        // This is not mapped to any braintree transaction status, this is in fact the transaction type: sale, credit or unrecognized
        public const int Refunded = 15;
    }
}
