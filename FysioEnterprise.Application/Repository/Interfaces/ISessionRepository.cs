namespace FysioEnterprise.Application.Repository.Interfaces
{
    public interface ISessionRepository
    {
        void CreateSession(Domain.Entities.Session session);
        Domain.Entities.Session GetSession(int ID);
        void UpdateSession(Domain.Entities.Session session);
        Domain.Entities.Session DeleteSession(int ID);

    }
}
