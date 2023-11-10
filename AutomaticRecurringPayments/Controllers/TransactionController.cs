using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using Azure;
using Braintree;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticRecurringPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : BaseController
    {
        public TransactionController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("pay")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateBraintreeTransactionResponse>> MakePayment([FromBody] CreateBraintreeTransactionCommand createBraintreeTransactionCommand, CancellationToken cancellationToken)
        {
            if (createBraintreeTransactionCommand == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var res = await Mediator.Send(createBraintreeTransactionCommand, cancellationToken);

            return Ok(res);
        }

        [HttpPost("refund")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefundBraintreeTransactionResponse>> RefundPayment([FromBody] RefundBraintreeTransactionCommand refundBraintreeTransactionCommand, CancellationToken cancellationToken)
        {
            if (refundBraintreeTransactionCommand == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var res = await Mediator.Send(refundBraintreeTransactionCommand, cancellationToken);

            return Ok(res);
        }
    }
}
