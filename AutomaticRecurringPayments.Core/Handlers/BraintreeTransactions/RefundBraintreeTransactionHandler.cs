using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayment.Model.BraintreeTransactions.Queries;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Handlers.BraintreeTransactions
{
    public class RefundBraintreeTransactionHandler : IRequestHandler<RefundBraintreeTransactionCommand, RefundBraintreeTransactionResponse>
    {
        private readonly IBraintreeTransactionService _braintreeTransactionService;
        private readonly IClientService _clientService;
        private readonly IBraintreeService _braintreeService;
        private readonly ISubscriptionService _subscriptionService;

        public RefundBraintreeTransactionHandler(
            IBraintreeTransactionService braintreeTransactionService,
            IClientService clientService,
            IBraintreeService braintreeService,
            ISubscriptionService subscriptionService
            )
        {
            _braintreeTransactionService = braintreeTransactionService;
            _clientService = clientService;
            _braintreeService = braintreeService;
            _subscriptionService = subscriptionService;
        }

        public async Task<RefundBraintreeTransactionResponse> Handle(
            RefundBraintreeTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var transaction = await _braintreeService.GetTransactionAsync(request.TransactionId);
            if (transaction == null) return null;

            BraintreeTransactionResponse transactionReponse = null;
            if (transaction.Status == Braintree.TransactionStatus.AUTHORIZED || 
                transaction.Status == Braintree.TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                transactionReponse = await _braintreeService.VoidTransactionAsync(request.TransactionId);
            }
            else if (transaction.Status == Braintree.TransactionStatus.SETTLED || 
                transaction.Status == Braintree.TransactionStatus.SETTLING)
            {
                transactionReponse = await _braintreeService.RefundTransactionAsync(request.TransactionId, (decimal)transaction.Amount);
            }
            else
            {
                transactionReponse = new BraintreeTransactionResponse
                {
                    Success = false,
                    Exception = new Exception("transaction: " + transaction?.Id 
                    + "with status " + transaction?.Status 
                    + " cannot be refunded or voided")
                };
            }

            var refundResponse = new RefundBraintreeTransactionResponse()
            {
                Success = transactionReponse.Success,
                ErrorMessage = transactionReponse.Exception?.Message,
            };

            return refundResponse;
        }
    }
}

