using AutomaticRecurringPayment.Model.Constants;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using AutomaticRecurringPayments.Core.Abstractions.Services;
using AutomaticRecurringPayments.Core.DatabaseContexts;
using AutomaticRecurringPayments.Core.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Services
{
    public class BraintreeTransactionService : IBraintreeTransactionService
    {
        private readonly DatabaseContext _databaseContext;

        public BraintreeTransactionService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<BraintreeTransaction> CreateAsync(BraintreeTransaction braintreeTransaction, CancellationToken cancellationToken = default)
        {
            return (await _databaseContext.BraintreeTransactions.AddAsync(braintreeTransaction, cancellationToken)).Entity;
        }

        public BraintreeTransaction Update(BraintreeTransaction braintreeTransaction)
        {
            return (_databaseContext.BraintreeTransactions.Update(braintreeTransaction)).Entity;
        }


        public async Task<BraintreeTransaction> GetByIdAsync(int id)
        {
            return (await _databaseContext.BraintreeTransactions.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<List<BraintreeTransaction>> GetAllAsync()
        {
            return await _databaseContext.BraintreeTransactions.ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _databaseContext.SaveChangesAsync();
        }
    }
}
