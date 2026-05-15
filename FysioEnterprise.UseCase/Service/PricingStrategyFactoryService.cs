using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.UseCase.Service
{
    public class PricingStrategyFactoryService : IPricingStrategyFactory
    {
        public IEnumerable<IPricingStrategy> BuildStrategies(
            LoyaltyLevel loyaltyLevel,
            bool isBirthdayMonth,
            Promotion? promotion)
        {
            var strategies = new List<IPricingStrategy>
        {
            new LoyaltyPricingStrategy(loyaltyLevel)
        };

            if (isBirthdayMonth)
                strategies.Add(new BirthdayPricingStrategy());

            if (promotion != null)
                strategies.Add(new PromotionPricingStrategy(promotion.PromotionDiscountPercent));

            return strategies;
        }
    }
}
