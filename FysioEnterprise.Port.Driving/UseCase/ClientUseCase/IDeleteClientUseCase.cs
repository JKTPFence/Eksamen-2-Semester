using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Port.Driving.UseCase.ClientCommands
{
    public interface IDeleteClientUseCase
    {
        Task<Result> DeleteClientAsync(DeleteClientRequest request);
    }
}
