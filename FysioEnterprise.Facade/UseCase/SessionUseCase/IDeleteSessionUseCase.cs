using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface IDeleteSessionUseCase
    {
        Task<Result> DeleteSessionAsync(DeleteSessionRequest request);
    }
}
