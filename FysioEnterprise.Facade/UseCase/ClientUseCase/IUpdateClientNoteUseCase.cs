using FluentResults;
using static FysioEnterprise.Facade.RequestModels.ClientRequests;

namespace FysioEnterprise.Facade.UseCase.ClientUseCase
{
    public interface IUpdateClientNoteUseCase
    {
        Task<Result> UpdateClientNoteAsync(UpdateClientNoteRequest request);
    }
}
