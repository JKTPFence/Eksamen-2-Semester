using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<Result> CreateClientAsync(Client client);
        Task<Result<Client>> GetClientAsync(Guid ID);
        Task<Result<List<Client>>> GetAllClientsAsync();
        Task<Result> UpdateClientAsync(Client client);
        Task<Result> DeleteClientAsync(Guid ID);
    }
}
