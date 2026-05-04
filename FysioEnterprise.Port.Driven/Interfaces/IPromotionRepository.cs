using FluentResults;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.UseCase.Repository.Interfaces
{
    public interface IPromotionRepository
    {
            Task<Result> CreatePromotionAsync(Promotion promotion);
            Task<Promotion> GetPromotionAsync(Guid ID);
            Task<Result> UpdatePromotionAsync(Promotion promotion);
            Task<Promotion> DeletePromotionAsync(Guid ID);
    }
}
