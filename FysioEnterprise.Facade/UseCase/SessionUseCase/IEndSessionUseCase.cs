using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface IEndSessionUseCase
    {
        Task<Result> EndSessionAsync(EndSessionRequest request);
    }
}
