using FysioEnterprise.Domain.Service.PricingService.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public class PriceCalculator
    {
        public decimal Calculate(decimal basePrice, IEnumerable<IPricingStrategy> strategies)
        {
            if (!strategies.Any())
                return basePrice;

            return strategies
                .Select(s => s.Apply(basePrice))
                .Min();
        }
    }
}
