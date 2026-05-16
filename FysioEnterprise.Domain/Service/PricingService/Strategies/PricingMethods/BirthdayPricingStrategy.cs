namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class BirthdayPricingStrategy : IPricingStrategy
    {
        public decimal Apply(decimal basePrice) => basePrice * 0.75m;
    }
}
