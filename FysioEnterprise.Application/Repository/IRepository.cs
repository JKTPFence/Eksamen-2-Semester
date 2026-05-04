using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.UseCase.Repository
{
    public interface ISessionRepository
    {
        Task<Session?> GetSessionAsync(Guid id);
        Task<IReadOnlyList<Session>> GetAllSessionsByClientAsync(Guid ClientID);
        Task<IReadOnlyList<Session>> GetAllSessionsByStaffAsync(Guid StaffID);
        Task AddAsync(Session session);
        Task SaveAsync();
        Task GetClientSessionsByClientIDAsync(object clientID);
        Task GetStaffSessionsByStaffIDAsync(object staffID);
    }

    public interface ISessionTypeRepository
    {
        Task GetByIDAsync(object sessionTypeID);
        Task<SessionType?> GetSessionTypeAsync(Guid id);
    }
    public interface IClientRepository
    {
        Task GetByIDAsync(object clientID);
        Task<Client?> GetClientAsync(Guid id);
    }
    public interface IStaffRepository
    {
        Task GetByIDAsync(object staffID);
        Task<Staff> GetStaffAsync(Guid id);
    }
}
