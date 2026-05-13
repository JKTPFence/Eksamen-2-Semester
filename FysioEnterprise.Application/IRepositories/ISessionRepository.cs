using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(Session session);
        Task<Result<Session>> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Session session);
        Task<Session> DeleteSessionAsync(Guid ID);
        Task<List<Session>> GetSessionsByClientAsync(Guid clientId);
        Task<List<Session>> GetSessionsByStaffAsync(Guid staffId);
        Task<List<Session>> GetCompletedSessionsInRangeAsync(DateTime from, DateTime to);

    }
}
