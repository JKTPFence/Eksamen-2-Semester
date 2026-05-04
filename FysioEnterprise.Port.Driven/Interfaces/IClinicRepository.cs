

using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IClinicRepository
    {
        Task<Result<Clinic>> GetClinicAsync(Guid ID);
    }
}
