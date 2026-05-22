using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class BirthdayPricingStrategy : IPricingStrategy
    {
        private double _discountPercentage = 25;
        public string Name => "Sessiontype price";

        public Price calculatePrice(Client client,
            Promotion? promotion,
            SessionType sessionType)
        {
            var discountAmount = sessionType.SessionTypePrice.Value * (_discountPercentage / 100);
            return new Price(discountAmount);
        }
    }
}
