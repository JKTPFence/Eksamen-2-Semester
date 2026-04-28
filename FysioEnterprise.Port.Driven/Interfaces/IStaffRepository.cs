using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IStaffRepository
    {
        Task<Domain.Entities.Staff> GetStaffAsync(Guid ID);
    }
}
