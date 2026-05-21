using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class StandardPricingStrategy : IPricingStrategy
    {
        public string Name => "Sessiontype price";

        public decimal calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            return sessionType.SessionTypePrice;
        }
    }
}
