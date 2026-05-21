using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies
{
    public interface IPricingStrategy
    {
        string Name { get; }
        decimal calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType);
    }
}
