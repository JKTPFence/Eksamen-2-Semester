using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class BirthdayPricingStrategy : IPricingStrategy
    {
        private decimal _discountPercentage = 20;
        public string Name => "Sessiontype price";

        public Price calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            return new Price(sessionType.SessionTypePrice);
        }
    }
}
