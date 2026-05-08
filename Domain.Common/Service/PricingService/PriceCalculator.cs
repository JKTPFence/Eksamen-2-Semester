using FysioEnterprise.Domain.Service.PricingService.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public class PriceCalculator
    {
        private readonly IEnumerable<IPricingStrategy> _strategies;

        public PriceCalculator(IEnumerable<IPricingStrategy> strategies)
        {
            _strategies = strategies;
        }

        public decimal Calculate(decimal basePrice)
        {
            if (!_strategies.Any())
                return basePrice;

            return _strategies
                .Select(strategy => strategy.Apply(basePrice))
                .Min(); 
        }
    }
}
