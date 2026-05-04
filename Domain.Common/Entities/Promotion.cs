using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Exceptions;

namespace FysioEnterprise.Domain.Entities
{
    public class Promotion
    {
        public Guid PromotionID { get; private set; }
        public string PromotionName { get; private set; }
        public decimal PromotionDiscountPercent { get; private set; }
        public DateTime PromotionStartTime { get; private set; }
        public DateTime PromotionEndTime { get; private set; }
        public ITimeNow TimeNow { get; private set; }
        public bool IsActive => IsPromotionActive(this);

        public Promotion(string promotionName, decimal promotionDiscountPercent, DateTime promotionStartTime, DateTime promotionEndTime, ITimeNow timeNow)
        {
            ValidatePromotionTime(promotionStartTime, promotionEndTime, timeNow);

            PromotionID = Guid.NewGuid();
            PromotionName = promotionName;
            PromotionDiscountPercent = promotionDiscountPercent;
            PromotionStartTime = promotionStartTime;
            PromotionEndTime = promotionEndTime;
            TimeNow = timeNow;
        }

        private static void ValidatePromotionTime(DateTime sessionStartTime, DateTime sessionEndTime, ITimeNow timeNow)
        {
            if (sessionStartTime >= sessionEndTime)
                throw new ArgumentException("Session must start before it ends.");
            if (sessionStartTime < timeNow.Now())
                throw new ArgumentException("Session start cannot be in the past.");
        }

        private static Boolean IsPromotionActive(Promotion promotion)
        {
            ValidatePromotionTime(promotion.PromotionStartTime, promotion.PromotionEndTime, promotion.TimeNow);
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
            ValidatePromotionTime(promotionStartTime, promotionEndTime, TimeNow);
            PromotionName = promotionName;
            PromotionDiscountPercent = promotionDiscountPercent;
            PromotionStartTime = promotionStartTime;
            PromotionEndTime = promotionEndTime;
        }
    }
}
