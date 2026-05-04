using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface ICreatePromotionUseCase
    {
        Task CreatePromotionAsync(CreatePromotionRequest request);
  
    }
}
