using AutomaticRecurringPayment.Model.BraintreeTransactions.Commands;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using Braintree;
using MediatR;

namespace AutomaticRecurringPayments.Core.Handlers.Subscriptions
{
    public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;

        public CancelSubscriptionHandler(
            ISubscriptionService subscriptionService
            )
        {
            _subscriptionService = subscriptionService;
        }

        public async Task<CancelSubscriptionResponse> Handle(
            CancelSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionService.GetByIdAsync(request.SubscriptionId);
            if (subscription == null)
                return null;
            if (subscription.ClientId != request.ClientId)
                return null;

            subscription.StatusId = (int)SubscriptionStatus.CANCELED;
            _subscriptionService.Update(subscription);
            await _subscriptionService.SaveChangesAsync();

            return new CancelSubscriptionResponse() { Canceled = true };
        }
    }
}