namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IRoomRepository
    {
        Task<Domain.Entities.Room> GetRoomAsync(Guid ID);
    }
}
