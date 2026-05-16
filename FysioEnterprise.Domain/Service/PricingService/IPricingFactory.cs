using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public interface IPricingStrategyFactory
    {
        IEnumerable<IPricingStrategy> BuildStrategies(
            LoyaltyLevel loyaltyLevel,
            bool isBirthdayMonth,
            Promotion? promotion);
    }
}
