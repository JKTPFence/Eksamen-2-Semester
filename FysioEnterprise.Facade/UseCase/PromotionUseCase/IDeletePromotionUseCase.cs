using FluentResults;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Facade.UseCase.PromotionUseCase
{
    public interface IDeletePromotionUseCase
    {
        Task<Result> DeletePromotionAsync(DeletePromotionRequest request);
    }
}
