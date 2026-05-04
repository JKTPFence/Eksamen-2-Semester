using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IUpdateClientUseCase
    {
        Task<Result> UpdateClientAsync(UpdateClientRequest request);
    }
}
