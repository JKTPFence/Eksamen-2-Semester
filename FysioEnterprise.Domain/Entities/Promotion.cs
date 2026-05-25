using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;

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
                throw new UserInvalidInputException($"En kampagne skal have et navn");
            PromotionName = promotionName;
            if (promotionDiscountPercent <= 0)
                throw new UserInvalidInputException($"En kampagne skal have en rabatprocent {promotionDiscountPercent}");
            PromotionDiscountPercent = promotionDiscountPercent;
                if (promotionStartTime >= promotionEndTime)
                    throw new UserInvalidInputException($"En kampagne skal have en starttid der er før sin sluttid {promotionStartTime} - {promotionEndTime}");
            PromotionStartTime = promotionStartTime;
            PromotionEndTime = promotionEndTime;
        }

        public static Promotion Create(
            string promotionName, 
            decimal promotionDiscountPercent, 
            DateTime promotionStartTime, 
            DateTime promotionEndTime)
        {
            var validationResult = TimeValidationService.ValidateTime(
                "Promotion",
                promotionStartTime,
                promotionEndTime,
                DateTime.Now);
            if (validationResult.IsFailed)
            {
                throw new ValidationException("Der er sket en fejl med tiden under oprettelsen af kampagnen " + validationResult.Errors);
            }
            var promotion = new Promotion(promotionName, promotionDiscountPercent, promotionStartTime, promotionEndTime);
            return promotion;
        }
        private static Boolean IsPromotionActive(Promotion promotion)
        {
            if (string.IsNullOrEmpty(promotion.PromotionName) || promotion.PromotionDiscountPercent <= 0)
            {
                throw new UserInvalidInputException("En kampagne skal have et navn og en rabatprocent");
            }
            
            if (DateTime.Now >= promotion.PromotionStartTime && DateTime.Now < promotion.PromotionEndTime)
            {
                return true;
            }
            return false;
        }
        public void UpdatePromotion(string promotionName, decimal promotionDiscountPercent, DateTime promotionStartTime, DateTime promotionEndTime)
        {
            var validationResult = TimeValidationService.ValidateTime(
                "Promotion",
                promotionStartTime,
                promotionEndTime,
                DateTime.Now);
            if (validationResult.IsFailed)
            {
                throw new ValidationException("Der er sket en fejl med tiden under oprettelsen af kampagnen " + validationResult.Errors);
            }

            if (string.IsNullOrWhiteSpace(promotionName))
                throw new UserInvalidInputException($"En kampagne skal have et navn");
            PromotionName = promotionName;
            if (promotionDiscountPercent <= 0)
                throw new UserInvalidInputException($"En kampagne skal have en rabatprocent {promotionDiscountPercent}");
            PromotionDiscountPercent = promotionDiscountPercent;
            PromotionStartTime = promotionStartTime;
            PromotionEndTime = promotionEndTime;
        }
    }
}
