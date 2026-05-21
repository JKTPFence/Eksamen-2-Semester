using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public interface IPricingStrategyFactory
    {
        Price BuildStrategies(
            Client client,
            Promotion? promotion,
            SessionType sessionType);
    }
}
