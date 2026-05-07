using FluentResults;
using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Facade.UseCase.PromotionUseCase
{
    public interface ICreatePromotionUseCase
    {
        Task<Result> CreatePromotionAsync(CreatePromotionRequest request);
  
    }
}
