using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Clients;
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
    public class ClientService : IClientService
    {
        private readonly DatabaseContext _databaseContext;

        public ClientService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Client> CreateAsync(Client client, CancellationToken cancellationToken)
        {
            return (await _databaseContext.Clients.AddAsync(client, cancellationToken)).Entity;
        }
        
        public Client Update(Client client)
        {
            return (_databaseContext.Clients.Update(client)).Entity;
        }

        public async Task<Client> GetByIdAsync(int clientId, CancellationToken cancellationToken)
        {
            return (await _databaseContext.Clients.FirstOrDefaultAsync(x => x.Id == clientId, cancellationToken));
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _databaseContext.SaveChangesAsync();
        }
    }
}
