using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class BirthdayPricingStrategy : IPricingStrategy
    {
        private const double DiscountPercentage = 25;
        public string Name => "Birthday Discount";

        public Price calculatePrice(Client client,
            Promotion? promotion,
            SessionType sessionType)
        {
            if (!client.IsBirthdayMonth(DateOnly.FromDateTime(DateTime.Today)))
                return new Price(0);

            if (client.HasUsedBirthdayDiscountThisYear)
                return new Price(0);

            var discountAmount = sessionType.SessionTypePrice.Value * (DiscountPercentage / 100);
            return new Price(discountAmount);
        }
    }
}
