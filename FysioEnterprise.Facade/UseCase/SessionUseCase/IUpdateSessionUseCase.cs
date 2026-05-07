using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface IUpdateSessionUseCase
    {
        Task<Result> UpdateSessionAsync(UpdateSessionRequest request);
    }
}
