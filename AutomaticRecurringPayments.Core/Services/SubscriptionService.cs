using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly DatabaseContext _databaseContext;

        public SubscriptionService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Subscription> CreateAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            return (await _databaseContext.Subscriptions.AddAsync(subscription, cancellationToken)).Entity;
        }

        public async Task<Subscription> GetByIdAsync(int id)
        {
            return await _databaseContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);
        }
        public Subscription Update(Subscription subscription)
        {
            return (_databaseContext.Subscriptions.Update(subscription)).Entity;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _databaseContext.SaveChangesAsync();
        }
    }
}
