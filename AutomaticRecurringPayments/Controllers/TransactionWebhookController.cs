using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticRecurringPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionWebhookController : BaseController
    {
        private readonly IBraintreeTransactionService _braintreeTransactionService;
        private readonly IBraintreeService _braintreeService;
        private readonly ISubscriptionService _subscriptionService;

        public TransactionWebhookController(
            IMediator mediator, 
            IBraintreeTransactionService braintreeTransactionService,
            IBraintreeService braintreeService,
            ISubscriptionService subscriptionService
            ) : base(mediator)
        {
            _braintreeTransactionService = braintreeTransactionService;
            _braintreeService = braintreeService;
            _subscriptionService = subscriptionService;
        }

        [HttpPost("verify-payment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyPayment(int id, bool c = false)
        {
            var braintreeTransaction = await _braintreeTransactionService.GetByIdAsync(id);
            if (braintreeTransaction == null)
                await Task.FromException(new Exception("braintreeTransaction Not Found"));

            var transaction = await _braintreeService.GetTransactionAsync(braintreeTransaction.TransactionId);
            if (transaction == null)
                return NotFound();

            var subscription = await _subscriptionService.GetByIdAsync(braintreeTransaction.SubscriptionId);

            return Ok(true);
        }
    }
}
