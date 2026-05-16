using FluentResults;
using static FysioEnterprise.Facade.RequestModels.SessionRequests;

namespace FysioEnterprise.Facade.UseCase.SessionUseCase
{
    public interface IMarkSessionAsNoShowUseCase
    {
        Task<Result> MarkSessionAsNoShowAsync(MarkNoShowSessionRequest request);
    }
}
