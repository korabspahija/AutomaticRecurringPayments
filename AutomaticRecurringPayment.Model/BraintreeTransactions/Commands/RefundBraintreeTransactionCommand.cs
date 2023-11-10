﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayment.Model.BraintreeTransactions.Commands
{
    public class RefundBraintreeTransactionCommand : IRequest<RefundBraintreeTransactionResponse>
    {
        public string TransactionId { get; set; }
    }
}
