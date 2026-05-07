using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface IRoomRepository
    {
        Task<Result<Room>> GetRoomAsync(Guid ID);
    }
}
