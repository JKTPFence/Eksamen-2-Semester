using FysioEnterprise.Facade.DTOs;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Port.Driving.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.QueryHandlers
{
    public class PromotionQueriesImpl : IPromotionQueries
    {
        private readonly AppDBContext _context;

        public PromotionQueriesImpl(AppDBContext context) => _context = context;

        public async Task<List<PromotionDTO>> GetAllPromotionsAsync()
        {
            return await _context.Promotions
                .AsNoTracking()
                .Select(p => new PromotionDTO(
                    p.PromotionID,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToListAsync();
        }

        public async Task<List<PromotionDTO>> GetAllActivePromotionsAsync()
        {
            return await _context.Promotions
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new PromotionDTO(
                    p.PromotionID,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToListAsync();
        }

        public async Task<List<PromotionDTO>> GetAllInActivePromotionsAsync()
        {
            return await _context.Promotions
                .AsNoTracking()
                .Where(p => !p.IsActive)
                .Select(p => new PromotionDTO(
                    p.PromotionID,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .ToListAsync();
        }

        public async Task<PromotionDTO?> GetPromotionByIdAsync(Guid promotionId)
        {
            return await _context.Promotions
                .AsNoTracking()
                .Where(p => p.PromotionID == promotionId)
                .Select(p => new PromotionDTO(
                    p.PromotionID,
                    p.PromotionName,
                    p.PromotionDiscountPercent,
                    p.PromotionStartTime,
                    p.PromotionEndTime,
                    p.IsActive))
                .FirstOrDefaultAsync();
        }
    }
}
