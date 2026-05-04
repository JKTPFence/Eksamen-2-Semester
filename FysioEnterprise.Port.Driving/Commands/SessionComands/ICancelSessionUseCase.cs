using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase
{
    public interface ICancelSessionUseCase
    {
        Task CancelSessionRequest(CancelSessionRequest request);
    }
}
