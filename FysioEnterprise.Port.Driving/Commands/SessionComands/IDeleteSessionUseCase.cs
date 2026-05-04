using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface IDeleteSessionUseCase
    {
        Task<Result> DeleteSessionAsync(DeleteSessionRequest request);
    }
}
