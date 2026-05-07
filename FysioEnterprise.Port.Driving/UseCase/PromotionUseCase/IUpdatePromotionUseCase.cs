using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.UseCase.PromotionCommands
{
    public interface IUpdatePromotionUseCase
    {
        Task UpdatePromotionAsync(UpdatePromotionRequest request);
    }
}
