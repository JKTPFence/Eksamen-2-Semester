using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface IUpdateSessionUseCase
    {
        Task<Result> UpdateSessionAsync(UpdateSessionRequest request);
    }
}
