using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface ICreateSessionUseCase
    {
        Task<Result> CreateSessionAsync(CreateSessionRequest request);

    }
}
