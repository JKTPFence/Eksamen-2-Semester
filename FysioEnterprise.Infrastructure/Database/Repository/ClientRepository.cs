using FluentResults;
using FysioEnterprise.UseCase.IRepositories;
using FysioEnterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<Client>> GetClientAsync(Guid clientId)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
                return Result.Fail($"Client with ID {clientId} was not found.");

            return Result.Ok(client);
        }

        public async Task<Result<List<Client>>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .AsNoTracking()
                .ToListAsync();

            if(clients.Any())
                return Result.Ok(clients);

            return Result.Fail("Operation failed, Could not get any clients");
        }

        public async Task<Result> UpdateClientAsync(Client client)
        {
            var emailExists = await _context.Clients
                .AnyAsync(c => c.ClientEmail == client.ClientEmail
                            && c.Id != client.Id);

            if (emailExists)
                return Result.Fail($"Email {client.ClientEmail} is already in use");

            var phoneExists = await _context.Clients
                .AnyAsync(c => c.ClientPhoneNumber == client.ClientPhoneNumber
                            && c.Id != client.Id);

            if (phoneExists)
                return Result.Fail($"Phonenumber {client.ClientPhoneNumber} is already in use");

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        
        public async Task<Result> DeleteClientAsync(Guid clientId)
        {
            var client = await GetClientAsync(clientId);

            if (client == null)
                return Result.Fail($"Client with ID {clientId} was not found.");

            _context.Clients.Remove(client.Value);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
