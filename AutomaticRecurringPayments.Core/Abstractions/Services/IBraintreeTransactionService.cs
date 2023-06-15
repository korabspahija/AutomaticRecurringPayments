using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Abstractions.Services
{
    public interface IBraintreeTransactionService
    {
        Task<BraintreeTransaction> CreateAsync(BraintreeTransaction braintreeTransaction, CancellationToken cancellationToken);
        BraintreeTransaction Update(BraintreeTransaction braintreeTransaction);
        Task<BraintreeTransaction> GetByIdAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
