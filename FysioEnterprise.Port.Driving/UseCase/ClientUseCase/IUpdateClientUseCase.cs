using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Port.Driving.UseCase.ClientCommands
{
    public interface IUpdateClientUseCase
    {
        Task<Result> UpdateClientAsync(UpdateClientRequest request);
    }
}
