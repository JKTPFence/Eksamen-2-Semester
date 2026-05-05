using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionCommands
{
    public interface ICancelSessionUseCase
    {
        Task CancelSessionRequest(CancelSessionRequest request);
    }
}
