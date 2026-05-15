using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class LoyaltyPricingStrategy : IPricingStrategy
    {
        private readonly LoyaltyLevel _loyaltyLevel;

        public LoyaltyPricingStrategy(LoyaltyLevel loyaltyLevel)
        {
            _loyaltyLevel = loyaltyLevel;
        }

        public decimal Apply(decimal basePrice)
            => basePrice * (1 - _loyaltyLevel.LoyaltyLevelDiscountPercentage / 100);
    }
}
