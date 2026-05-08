using FysioEnterprise.Domain.Service;

namespace FysioEnterprise.Domain.Entities
{
    public class Promotion
    {
        public Guid PromotionID { get; private set; }
        public string PromotionName { get; private set; }
        public decimal PromotionDiscountPercent { get; private set; }
        public DateTime PromotionStartTime { get; private set; }
        public DateTime PromotionEndTime { get; private set; }
        public bool IsActive => IsPromotionActive(this);

        public static Promotion Create(
            string promotionName, 
            decimal promotionDiscountPercent, 
            DateTime promotionStartTime, 
            DateTime promotionEndTime)
        {
            return new Promotion
            {
                PromotionID = Guid.NewGuid(),
                PromotionName = promotionName,
                PromotionDiscountPercent = promotionDiscountPercent,
                PromotionStartTime = promotionStartTime,
                PromotionEndTime = promotionEndTime,
            };
        }
        private static Boolean IsPromotionActive(Promotion promotion)
        {
            if (string.IsNullOrEmpty(promotion.PromotionName) || promotion.PromotionDiscountPercent <= 0)
            {
                throw new ArgumentNullException("Promotion needs to have a name and a discount");
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
