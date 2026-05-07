using static FysioEnterprise.Facade.RequestModels.PromotionRequests;

namespace FysioEnterprise.Port.Driving.UseCase.PromotionCommands
{
    public interface IDeletePromotionUseCase
    {
        Task DeletePromotionAsync(DeletePromotionRequest request);
    }
}
