using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(Session session);
        Task<Result<Session>> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Session session);
        Task SaveChangesAsync();
        Task<Session> DeleteSessionAsync(Guid ID);
        Task<List<Session>> GetSessionsByClientAsync(Guid clientId, Guid? excludeSessionId = null);
        Task<List<Session>> GetSessionsByStaffAsync(Guid staffId, Guid? excludeSessionId = null);
        Task<List<Session>> GetSessionsByRoomAsync(Guid clinicId, Guid roomId, Guid? excludeSessionId = null);
        Task<List<Session>> GetSessionsInRangeAsync(DateTime from, DateTime to);

    }
}
