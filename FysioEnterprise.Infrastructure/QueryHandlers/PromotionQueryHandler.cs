using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Facade.Queries;
using Microsoft.EntityFrameworkCore;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class PromotionQueriesImpl : IPromotionQueries
    {
        private readonly AppDBContext _context;

        public PromotionQueriesImpl(AppDBContext context) => _context = context;

        public async Task<List<PromotionDTO>> GetAllPromotionsAsync()
        {
            var promotions = await _context.Promotions
                .AsNoTracking()
                .ToListAsync();

            return promotions
                .Select(p => new PromotionDTO(
                    p.Id,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToList();
        }

        public async Task<List<PromotionDTO>> GetAllActivePromotionsAsync()
        {
            var promotions = await _context.Promotions
                .AsNoTracking()
                .ToListAsync();

            return promotions
                .Where(p => p.IsActive)
                .Select(p => new PromotionDTO(
                    p.Id,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToList();
        }

        public async Task<List<PromotionDTO>> GetAllInActivePromotionsAsync()
        {
            var promotions = await _context.Promotions
                .AsNoTracking()
                .ToListAsync();

            return promotions
                .Where(p => !p.IsActive)
                .Select(p => new PromotionDTO(
                    p.Id,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToList();
        }

        public async Task<PromotionDTO?> GetPromotionByIdAsync(Guid promotionId)
        {
            var promotions = await _context.Promotions
                 .AsNoTracking()
                 .ToListAsync();

            return promotions
                .Where(p => p.Id == promotionId)
                .Select(p => new PromotionDTO(
                    p.Id,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .FirstOrDefault();
        }
    }
}
