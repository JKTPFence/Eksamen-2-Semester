using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionCommands
{
    public interface IEndSessionUseCase
    {
        Task EndSessionRequest(EndSessionRequest request);
    }
}
