using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using AutomaticRecurringPayments.Core.DatabaseContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Abstractions.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken);
        Task<Subscription> GetByIdAsync(int id);
        Subscription Update(Subscription subscription);
        Task<int> SaveChangesAsync();
    }
}
