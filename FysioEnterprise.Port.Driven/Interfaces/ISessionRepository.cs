using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(Session session);
        Task<Result<Session>> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Session session);
        Task<Session> DeleteSessionAsync(Guid ID);
        Task<List<Session>> GetSessionsByClientAsync(Guid clientId);
        Task<List<Session>> GetSessionsByStaffAsync(Guid staffId);

    }
}
