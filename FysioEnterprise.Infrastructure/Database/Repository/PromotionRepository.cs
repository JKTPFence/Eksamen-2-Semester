using FysioEnterprise.Domain.Entities;
using FysioEnterprise.UseCase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.Database.Repository
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDBContext _context;

        public PromotionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreatePromotionAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<Promotion> GetPromotionAsync(Guid promotionId)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionID == promotionId);

            if (promotion == null)
                throw new KeyNotFoundException($"Promotion with ID {promotionId} was not found.");

            return promotion;
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<Promotion> DeletePromotionAsync(Guid promotionId)
        {
            var promotion = await GetPromotionAsync(promotionId);
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }
    }
}
