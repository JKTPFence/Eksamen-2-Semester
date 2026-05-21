using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class StandardPricingStrategy : IPricingStrategy
    {
        public string Name => "Sessiontype price";

        public Price calculatePrice(Client client,
            Promotion? promotion,
            SessionType sessionType)
        {
            return sessionType.SessionTypePrice;
        }
    }
}
