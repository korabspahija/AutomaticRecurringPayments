using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticRecurringPayments.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : BaseController
    {
        public SubscriptionController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MakePayment([FromBody] CancelSubscriptionCommand cancelSubscriptionCommand, CancellationToken cancellationToken)
        {
            if (cancelSubscriptionCommand == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var res = await Mediator.Send(cancelSubscriptionCommand, cancellationToken);

            return Ok(true);
        }
    }
}
