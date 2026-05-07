using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface ICreateSessionUseCase
    {
        Task<Result> CreateSessionAsync(CreateSessionRequest request);

    }
}
