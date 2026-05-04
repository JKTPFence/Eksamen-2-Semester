using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IPromotionRepository
    {
            Task CreatePromotionAsync(Domain.Entities.Promotion promotion);
            Task<Domain.Entities.Promotion> GetPromotionAsync(Guid ID);
            Task UpdatePromotionAsync(Domain.Entities.Promotion promotion);
            Task<Domain.Entities.Promotion> DeletePromotionAsync(Guid ID);
            Task AddPromotionAsync(Promotion promotion);
    }
}
