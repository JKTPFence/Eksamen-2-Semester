using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Port.Driving.UseCase.ClientCommands
{
    public interface ICreateClientUseCase
    {
        Task<Result> CreateClientAsync(CreateClientRequest command);
    }
}
