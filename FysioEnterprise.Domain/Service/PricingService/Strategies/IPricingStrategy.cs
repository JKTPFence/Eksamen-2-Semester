using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies
{
    public interface IPricingStrategy
    {
        decimal Apply(decimal basePrice);
    }
}
