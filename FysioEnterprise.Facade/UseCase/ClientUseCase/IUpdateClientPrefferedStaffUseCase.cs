using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Facade.UseCase.ClientUseCase
{
    public interface IUpdateClientPrefferedStaffUseCase
    {
        Task<Result> UpdateClientPrefferedStaffAsync(UpdateClientStaffRequest requet);
    }
}
