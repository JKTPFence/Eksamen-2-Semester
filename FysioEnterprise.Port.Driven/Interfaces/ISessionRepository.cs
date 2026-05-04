using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;

namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface ISessionRepository
    {
        Task<Result> CreateSessionAsync(Session session);
        Task<Session> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Session session);
        Task<Session> DeleteSessionAsync(Guid ID);
        Task<List<Session>> GetSessionsByClientAsync(Guid clientId);
        Task<List<Session>> GetSessionsByStaffAsync(Guid staffId);

    }
}
