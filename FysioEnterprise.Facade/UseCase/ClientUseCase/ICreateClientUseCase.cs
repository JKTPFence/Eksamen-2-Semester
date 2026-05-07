using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Facade.UseCase.ClientUseCase
{
    public interface ICreateClientUseCase
    {
        Task<Result> CreateClientAsync(CreateClientRequest request);
    }
}
