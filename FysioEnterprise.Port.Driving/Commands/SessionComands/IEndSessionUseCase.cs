using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase
{
    public interface IEndSessionUseCase
    {
        Task EndSessionRequest(EndSessionRequest request);
    }
}
