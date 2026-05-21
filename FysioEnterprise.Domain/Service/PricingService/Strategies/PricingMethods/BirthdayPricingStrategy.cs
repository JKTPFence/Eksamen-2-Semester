using System.Security.Cryptography.X509Certificates;
using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods
{
    public class BirthdayPricingStrategy : IPricingStrategy
    {
        private decimal _discountPercentage = 20;
        public string Name => "Sessiontype price";

        public decimal calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType)
        {
            return sessionType.SessionTypePrice;
        }
    }
}
