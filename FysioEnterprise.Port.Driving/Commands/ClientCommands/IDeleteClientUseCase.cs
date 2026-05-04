using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Port.Driving.Commands.ClientCommands
{
    public interface IDeleteClientUseCase
    {
        Task<Result> DeleteClientAsync(DeleteClientRequest request);
    }
}
