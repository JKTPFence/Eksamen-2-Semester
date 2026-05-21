using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService.Strategies;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public class PriceCalculator : IPricingStrategyFactory
    {
        private readonly IEnumerable<IPricingStrategy> _strategies;

        public PriceCalculator(IEnumerable<IPricingStrategy> strategies)
        {
            _strategies = strategies;
        }
        public async Task<decimal> BuildStrategies(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            var rabatter = _rabatStrategier.Select(a => a.Beregn(tidspunkt, behandlingstype, patient));
            var bedsteRabat = rabatter.MaxBy(a => a.Beløb) ?? new BeregnetRabat(0);

            var resultatBeløb = behandlingstype.EgenbetalingsBeløb.Beløb - bedsteRabat.Beløb;

            resultatBeløb = Math.Max(0, resultatBeløb);

            return new EgenbetalingsBeløb(resultatBeløb);
        }
    }
}
