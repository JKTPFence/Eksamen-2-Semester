using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.Commands.PromotionCommands
{
    public interface IDeletePromotionUseCase
    {
        Task DeletePromotionAsync(DeletePromotionRequest request);
    }
}
