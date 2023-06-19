using AutomaticRecurringPayment.Model.Entities.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.Abstractions.Services
{
    public interface IClientService
    {
        Task<Client> CreateAsync(Client client, CancellationToken cancellationToken);
        Client Update(Client client);
        Task<Client> GetByIdAsync(int clientId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
