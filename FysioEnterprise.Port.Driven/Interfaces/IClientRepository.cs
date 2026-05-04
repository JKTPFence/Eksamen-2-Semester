using FluentResults;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task<Result> CreateClientAsync(Domain.Entities.Client client);
        Task<Domain.Entities.Client> GetClientAsync(Guid ID);
        Task<List<Domain.Entities.Client>> GetAllClientsAsync();
        Task<Result> UpdateClientAsync(Domain.Entities.Client client);
        Task<Domain.Entities.Client> DeleteClientAsync(Guid ID);

    }
}
