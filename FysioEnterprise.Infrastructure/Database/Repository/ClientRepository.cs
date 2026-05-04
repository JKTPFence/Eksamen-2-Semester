using FluentResults;
using FysioEnterprise.Application.Repository.Interfaces;
using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<Result> CreateClientAsync(Client client)
        {
            var exists = await _context.Clients
                .AnyAsync(c => c.ClientEmail == client.ClientEmail);

            if (exists)
                return Result.Fail($"Client with email {client.ClientEmail} already exists");

            try
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail($"Could not save client: {ex.Message}");
            }
        }

        public async Task<Client> GetClientAsync(Guid clientId)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientID == clientId);

            if (client == null)
                throw new KeyNotFoundException($"Client with ID {clientId} was not found.");

            return client;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .AsNoTracking()
                .ToListAsync();
            return clients;
        }

        public async Task<Result> UpdateClientAsync(Client client)
        {
            var emailExists = await _context.Clients
                .AnyAsync(c => c.ClientEmail == client.ClientEmail
                            && c.ClientID != client.ClientID);

            if (emailExists)
                return Result.Fail($"Email {client.ClientEmail} is already in use");

            var phoneExists = await _context.Clients
                .AnyAsync(c => c.ClientPhoneNumber == client.ClientPhoneNumber
                            && c.ClientID != client.ClientID);

            if (phoneExists)
                return Result.Fail($"Phonenumber {client.ClientPhoneNumber} is already in use");

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return Result.Ok();
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
