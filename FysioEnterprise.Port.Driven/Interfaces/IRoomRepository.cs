using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IRoomRepository
    {
        Task<Result<Room>> GetRoomAsync(Guid ID);
    }
}
