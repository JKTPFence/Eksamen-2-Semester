using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IClinicRepository
    {
        Task<Domain.Entities.Clinic> GetClinicAsync(Guid ID);
    }
}
