using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDBContext _context;

        public ClientRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreateClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task<Client> GetClientAsync(Guid clientId)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientID == clientId);

            if (client == null)
                throw new KeyNotFoundException($"Client with ID {clientId} was not Found.");

            return client;
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Client> DeleteClientAsync(Guid clientId)
        {
            var client = await GetClientAsync(clientId);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return client;
        }
    }
}
