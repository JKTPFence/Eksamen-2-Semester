using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface ISessionTypeRepository
    {
        Task<Result<SessionType>> GetSessionTypeAsync(Guid ID);
    }
}
