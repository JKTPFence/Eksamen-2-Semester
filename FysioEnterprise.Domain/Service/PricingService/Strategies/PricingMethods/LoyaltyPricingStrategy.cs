using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class LoyaltyPricingStrategy : IPricingStrategy
    {
        public string Name => "Loyalty Level Discount";

        public Price calculatePrice(Client client,
            Promotion? promotion,
            SessionType sessionType)
        {
            var discount = sessionType.SessionTypePrice.Value * Convert.ToDouble(client.ClientLoyaltyLevel.LoyaltyLevelDiscountPercentage / 100);
            return new Price(discount);
        }
    }
}
