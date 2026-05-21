using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.IRepositories
{
    public interface IPromotionRepository
    {
            Task<Result> CreatePromotionAsync(Promotion promotion);
            Task<Result<Promotion>> GetPromotionAsync(Guid ID);
            Task<Result> UpdatePromotionAsync(Promotion promotion);
            Task<Promotion> DeletePromotionAsync(Guid ID);
    }
}
