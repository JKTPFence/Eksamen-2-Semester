using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface IClientRepository
    {
        Task CreateClientAsync(Domain.Entities.Client client);
        Task<Domain.Entities.Client> GetClientAsync(Guid ID);
        Task UpdateClientAsync(Domain.Entities.Client client);
        Task<Domain.Entities.Client> DeleteClientAsync(Guid ID);
        Task AddClientAsync(Client client);
        Task GetClientByEmailAsync(string email);
    }
}
