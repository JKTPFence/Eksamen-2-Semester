using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.UseCase.PromotionCommands
{
    public interface ICreatePromotionUseCase
    {
        Task CreatePromotionAsync(CreatePromotionRequest request);
  
    }
}
