using AutomaticRecurringPayment.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Helpers
{
    public static class TransactionStatusHelper
    {
        public static int PrepareBraintreeTransactionStatus(Braintree.TransactionStatus? transactionStatus)
        {
            return transactionStatus switch
            {
                Braintree.TransactionStatus.AUTHORIZATION_EXPIRED => BraintreeTransactionStatusConstants.AUTHORIZATION_EXPIRED,
                Braintree.TransactionStatus.AUTHORIZED => BraintreeTransactionStatusConstants.AUTHORIZED,
                Braintree.TransactionStatus.AUTHORIZING => BraintreeTransactionStatusConstants.AUTHORIZING,
                Braintree.TransactionStatus.FAILED => BraintreeTransactionStatusConstants.FAILED,
                Braintree.TransactionStatus.GATEWAY_REJECTED => BraintreeTransactionStatusConstants.GATEWAY_REJECTED,
                Braintree.TransactionStatus.PROCESSOR_DECLINED => BraintreeTransactionStatusConstants.PROCESSOR_DECLINED,
                Braintree.TransactionStatus.SETTLED => BraintreeTransactionStatusConstants.SETTLED,
                Braintree.TransactionStatus.SETTLING => BraintreeTransactionStatusConstants.SETTLING,
                Braintree.TransactionStatus.SUBMITTED_FOR_SETTLEMENT => BraintreeTransactionStatusConstants.SUBMITTED_FOR_SETTLEMENT,
                Braintree.TransactionStatus.VOIDED => BraintreeTransactionStatusConstants.VOIDED,
                Braintree.TransactionStatus.UNRECOGNIZED => BraintreeTransactionStatusConstants.UNRECOGNIZED,
                Braintree.TransactionStatus.SETTLEMENT_CONFIRMED => BraintreeTransactionStatusConstants.SETTLEMENT_CONFIRMED,
                Braintree.TransactionStatus.SETTLEMENT_DECLINED => BraintreeTransactionStatusConstants.SETTLEMENT_DECLINED,
                Braintree.TransactionStatus.SETTLEMENT_PENDING => BraintreeTransactionStatusConstants.SETTLEMENT_PENDING,
                _ => default,
            };
        }
    }
}
