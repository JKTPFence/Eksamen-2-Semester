using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service.PricingService.Strategies
{
    public interface IPricingStrategy
    {
        string Name { get; }
        Price calculatePrice(Client client,
            Promotion promotion,
            SessionType sessionType);
    }
}
