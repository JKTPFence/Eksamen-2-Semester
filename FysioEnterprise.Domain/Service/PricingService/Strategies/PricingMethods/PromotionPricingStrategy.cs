using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class PromotionPricingStrategy : IPricingStrategy
    {
        private readonly decimal _discountPercentage;

        public PromotionPricingStrategy(decimal discountPercentage)
        {
            _discountPercentage = discountPercentage;
        }

        public decimal Apply(decimal basePrice) => basePrice * (1 - _discountPercentage / 100);
    }
}
