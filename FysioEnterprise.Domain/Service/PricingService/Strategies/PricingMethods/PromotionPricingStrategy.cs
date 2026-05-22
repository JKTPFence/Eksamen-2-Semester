using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class PromotionPricingStrategy : IPricingStrategy
    {
        public string Name => "Promotion Discount";

        public Price calculatePrice(Client client,
            Promotion? promotion,
            SessionType sessionType)
        {
            if (promotion is null || !promotion.IsActive)
                return new Price(0);

            var savings = sessionType.SessionTypePrice.Value * Convert.ToDouble(promotion.PromotionDiscountPercent / 100);
            return new Price(savings);
        }
    }
}
