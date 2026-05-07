using FluentResults;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Facade.UseCase.PromotionUseCase
{
    public interface IUpdatePromotionUseCase
    {
        Task<Result> UpdatePromotionAsync(UpdatePromotionRequest request);
    }
}
