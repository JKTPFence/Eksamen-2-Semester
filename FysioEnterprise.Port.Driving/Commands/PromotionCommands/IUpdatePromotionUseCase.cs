using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface IUpdatePromotionUseCase
    {
        Task UpdatePromotionAsync(UpdatePromotionRequest request);
    }
}
