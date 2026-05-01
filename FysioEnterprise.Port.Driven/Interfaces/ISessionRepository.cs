using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(Session session);
        Task<Session> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Session session);
        Task<Session> DeleteSessionAsync(Guid ID);
        Task<List<Session>> GetSessionsByClientIdAsync(Guid clientId);
    }
}
