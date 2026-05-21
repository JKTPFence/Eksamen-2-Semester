using FluentResults;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.UseCase.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDBContext _context;

        public PromotionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Result> CreatePromotionAsync(Promotion promotion)
        {
            var exists = await _context.Promotions
                .AnyAsync(p => p.PromotionName == promotion.PromotionName);

            if (exists)
                return Result.Fail($"Promotion with name {promotion.PromotionName} already exists");

            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result<Promotion>> GetPromotionAsync(Guid promotionId)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Id == promotionId);

            if (promotion == null)
                return Result.Fail<Promotion>($"Promotion with ID {promotionId} was not found.");

            return Result.Ok(promotion);
        }

        public async Task<Result> UpdatePromotionAsync(Promotion promotion)
        {
            var exists = await _context.Promotions
                .AnyAsync(p => p.PromotionName == promotion.PromotionName
                            && p.Id != promotion.Id);

            if (exists)
                return Result.Fail($"Promotion with name {promotion.PromotionName} already exists");

            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Promotion> DeletePromotionAsync(Guid promotionId)
        {
            var promotion = await GetPromotionAsync(promotionId);
            _context.Promotions.Remove(promotion.Value);
            await _context.SaveChangesAsync();
            return promotion.Value;
        }
    }
}
