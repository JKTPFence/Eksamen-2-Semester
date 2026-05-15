using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal Apply(decimal basePrice) => basePrice;
    }
}
