using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService
{
   public class PriceCalculator : IPricingStrategyFactory
    {
        private readonly IEnumerable<IPricingStrategy> _strategies;
        private readonly object _lock = new object();

        public PriceCalculator(IEnumerable<IPricingStrategy> strategies)
        {
            _strategies = strategies;
        }
        public Task<Price> BuildStrategies(Client client,
                Promotion? promotion,
                SessionType sessionType)
        {
            Price bestDiscount = new Price(0);

            Parallel.ForEach(_strategies, s =>
            {
                var discount = s.calculatePrice(client, promotion, sessionType);

                lock (_lock)
                {
                    if (discount != null && discount.Value > bestDiscount.Value)
                    {
                        bestDiscount = discount;
                    }
                }
            });

            var result = bestDiscount.Value > 0
                ? Math.Max(0, sessionType.SessionTypePrice.Value - bestDiscount.Value)
                : sessionType.SessionTypePrice.Value;

            return Task.FromResult(new Price(result));
        }
    }
}
