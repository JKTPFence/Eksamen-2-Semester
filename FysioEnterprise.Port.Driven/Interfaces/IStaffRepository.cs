using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IStaffRepository
    {
        Task<Result<Staff>> GetStaffAsync(Guid ID);
    }
}
