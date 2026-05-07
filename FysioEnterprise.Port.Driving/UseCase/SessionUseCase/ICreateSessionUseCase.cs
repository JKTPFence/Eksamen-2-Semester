using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Port.Driving.UseCase.SessionCommands
{
    public interface ICreateSessionUseCase
    {
        Task<Result> CreateSessionAsync(CreateSessionRequest request);

    }
}
