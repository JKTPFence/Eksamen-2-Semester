using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface ICancelSessionUseCase
    {
        Task<Result> CancelSessionAsync(CancelSessionRequest request);
    }
}
