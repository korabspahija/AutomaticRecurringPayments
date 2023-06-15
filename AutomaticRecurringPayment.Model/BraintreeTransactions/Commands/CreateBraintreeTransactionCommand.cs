using MediatR;
using Newtonsoft.Json;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Commands
{
    public class CreateBraintreeTransactionCommand : IRequest<CreateBraintreeTransactionResponse>
    {
        public string Nonce { get; set; }
        public string DeviceData { get; set; }
        public bool IsRecurring { get; set; }
        public int ClientId { get; set; }
    }
}
