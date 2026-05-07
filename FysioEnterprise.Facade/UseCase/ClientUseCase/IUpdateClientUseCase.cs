using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Facade.UseCase.ClientUseCase
{
    public interface IUpdateClientUseCase
    {
        Task<Result> UpdateClientAsync(UpdateClientRequest request);
    }
}
