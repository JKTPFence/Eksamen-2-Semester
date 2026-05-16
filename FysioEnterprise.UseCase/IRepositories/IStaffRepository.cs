using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface IStaffRepository
    {
        Task<Result<Staff>> GetStaffAsync(Guid ID);
    }
}
