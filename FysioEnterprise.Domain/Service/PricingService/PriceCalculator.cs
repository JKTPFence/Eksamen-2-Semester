using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService
{
   /* public class PriceCalculator : IPricingStrategyFactory
    {
        private readonly IEnumerable<IPricingStrategy> _strategies;

        public PriceCalculator(IEnumerable<IPricingStrategy> strategies)
        {
            _strategies = strategies;
        }
        public Price BuildStrategies(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            var discounts = _strategies.Select(a => a.calculatePrice(client, promotion, sessionType));
            var bestDiscount = discounts.MaxBy(a => a.Value) ?? new Price(0);

            var result = sessionType.SessionTypePrice - bestDiscount.Value;

            result = Math.Max(0, result);

            return new Price(result);
        }
    }*/
}
