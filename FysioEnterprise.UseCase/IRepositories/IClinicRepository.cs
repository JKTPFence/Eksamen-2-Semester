

using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface IClinicRepository
    {
        Task<Result<Clinic>> GetClinicAsync(Guid ID);
    }
}
