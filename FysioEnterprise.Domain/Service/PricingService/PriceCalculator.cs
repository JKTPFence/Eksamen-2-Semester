using FysioEnterprise.Domain.Service.PricingService.Strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService
{
    public class PriceCalculator
    {
        public async Task<decimal> Calculate(decimal basePrice, IEnumerable<IPricingStrategy> strategies)
        {
            if (!strategies.Any())
                return basePrice;

            var results = await Task.WhenAll(
                strategies.Select(s => Task.Run(() => s.Apply(basePrice))));

            return results.Min();
        }
    }
}
