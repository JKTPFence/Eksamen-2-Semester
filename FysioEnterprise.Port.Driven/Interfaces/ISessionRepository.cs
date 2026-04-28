namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(Domain.Entities.Session session);
        Task<Domain.Entities.Session> GetSessionAsync(Guid ID);
        Task UpdateSessionAsync(Domain.Entities.Session session);
        Task<Domain.Entities.Session> DeleteSessionAsync(Guid ID);

    }
}
