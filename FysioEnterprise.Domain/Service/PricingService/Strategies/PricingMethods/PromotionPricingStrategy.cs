using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class PromotionPricingStrategy : IPricingStrategy
    {
        private decimal _discountPercentage;
        public string Name => "Promotion Discount";

        public decimal calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            _discountPercentage = promotion.PromotionDiscountPercent;
            return sessionType.SessionTypePrice * (1 - _discountPercentage / 100);
        }
    }
}
