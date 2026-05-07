using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface ICancelSessionUseCase
    {
        Task CancelSessionRequest(CancelSessionRequest request);
    }
}
