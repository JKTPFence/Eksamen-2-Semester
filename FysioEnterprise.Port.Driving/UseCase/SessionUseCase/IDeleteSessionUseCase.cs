using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Port.Driving.UseCase.SessionCommands
{
    public interface IDeleteSessionUseCase
    {
        Task<Result> DeleteSessionAsync(DeleteSessionRequest request);
    }
}
