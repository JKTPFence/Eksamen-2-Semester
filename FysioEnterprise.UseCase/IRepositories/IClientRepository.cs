using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
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
