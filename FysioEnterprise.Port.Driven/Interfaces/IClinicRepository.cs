

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IClinicRepository
    {
        Task<Domain.Entities.Clinic> GetClinicAsync(Guid ID);
    }
}
