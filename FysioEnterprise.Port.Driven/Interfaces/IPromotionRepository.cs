using FluentResults;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IPromotionRepository
    {
            Task<Result> CreatePromotionAsync(Domain.Entities.Promotion promotion);
            Task<Domain.Entities.Promotion> GetPromotionAsync(Guid ID);
            Task<Result> UpdatePromotionAsync(Domain.Entities.Promotion promotion);
            Task<Domain.Entities.Promotion> DeletePromotionAsync(Guid ID);
    }
}
