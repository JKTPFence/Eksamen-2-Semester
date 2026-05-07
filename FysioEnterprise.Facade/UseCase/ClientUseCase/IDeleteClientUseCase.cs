using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Facade.UseCase.ClientUseCase
{
    public interface IDeleteClientUseCase
    {
        Task<Result> DeleteClientAsync(DeleteClientRequest request);
    }
}
