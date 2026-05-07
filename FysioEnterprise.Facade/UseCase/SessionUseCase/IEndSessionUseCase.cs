using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface IEndSessionUseCase
    {
        Task EndSessionRequest(EndSessionRequest request);
    }
}
