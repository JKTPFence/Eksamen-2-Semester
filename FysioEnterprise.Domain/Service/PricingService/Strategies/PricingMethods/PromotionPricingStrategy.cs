using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class PromotionPricingStrategy : IPricingStrategy
    {
        private decimal _discountPercentage;
        public string Name => "Promotion Discount";

        public Price calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            _discountPercentage = promotion.PromotionDiscountPercent;
            return new Price(sessionType.SessionTypePrice * (1 - _discountPercentage / 100));
        }
    }
}
