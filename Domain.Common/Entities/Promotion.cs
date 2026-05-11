using FysioEnterprise.Domain.Exceptions;

namespace FysioEnterprise.Domain.Entities
{
    public class Promotion : Aggregateroot
    {
        public string PromotionName { get; private set; }
        public decimal PromotionDiscountPercent { get; private set; }
        public DateTime PromotionStartTime { get; private set; }
        public DateTime PromotionEndTime { get; private set; }
        public bool IsActive => IsPromotionActive(this);

        Promotion() { } // Empty constructor for EF Core

        private Promotion(
            string promotionName, 
            decimal promotionDiscountPercent, 
            DateTime promotionStartTime, 
            DateTime promotionEndTime)
        {
            Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(promotionName))
                throw new DomainException($"Promotion name cannot be empty: {promotionName}");
            if (promotionDiscountPercent <= 0)
                throw new DomainException($"Discount percentage must be greater than zero: {promotionDiscountPercent}");
        }

        public static Promotion Create(
            string promotionName, 
            decimal promotionDiscountPercent, 
            DateTime promotionStartTime, 
            DateTime promotionEndTime)
        { 
            var promotion = new Promotion(promotionName, promotionDiscountPercent, promotionStartTime, promotionEndTime);
            return promotion;
        }
        private static Boolean IsPromotionActive(Promotion promotion)
        {
            if (string.IsNullOrEmpty(promotion.PromotionName) || promotion.PromotionDiscountPercent <= 0)
            {
                throw new DomainException("Promotion needs to have a name and a discount");
            }
            
            if (DateTime.Now >= promotion.PromotionStartTime && DateTime.Now < promotion.PromotionEndTime)
            {
                return true;
            }
            return false;
        }
        public void UpdatePromotion(string promotionName, decimal promotionDiscountPercent, DateTime promotionStartTime, DateTime promotionEndTime)
        {
            PromotionName = promotionName;
            PromotionDiscountPercent = promotionDiscountPercent;
            PromotionStartTime = promotionStartTime;
            PromotionEndTime = promotionEndTime;
        }
    }
}
