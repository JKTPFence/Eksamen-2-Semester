using System.Diagnostics;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Domain.ValueObjects;
using Xunit;
using Xunit.Abstractions;
namespace FysioEnterprise.Domain.Tests
{
    public class PriceCalculatorTests
    {
        private static readonly Price BasePriceValue = new Price(100D);
        private readonly ITestOutputHelper _output;

        private (Client client, SessionType sessionType) CreateBaseDomainContext(
            LoyaltyLevel loyaltyLevel,
            DateOnly? birthDate = null)
        {
            var sessionType = new SessionType(
                "Standard Session",
                BasePriceValue,
                4,
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
                new List<int>()
            );

            var client = Client.Create(
                "Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                birthDate ?? new DateOnly(1995, 6, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: loyaltyLevel
            );

            return (client, sessionType);
        }

        [Fact]
        public async Task Calculate_NoStrategies_ReturnsBasePrice()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.None);
            var calculator = new PriceCalculator(new List<IPricingStrategy>());

            var result = await calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(BasePriceValue.Value, result.Value);
        }

        [Fact]
        public async Task Calculate_OnlyLoyalty_AppliesLoyaltyDiscount()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Gold);
            var calculator = new PriceCalculator(new List<IPricingStrategy> { new LoyaltyPricingStrategy() });

            var result = await calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(85D, result.Value);
        }

        [Fact]
        public async Task Calculate_BirthdayBeatLoyalty_AppliesBirthdayDiscount()
        {
            var birthday = new DateOnly(1995, 6, 1);
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Bronze, birthday);

            var calculator = new PriceCalculator(new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new BirthdayPricingStrategy()
            });

            var result = await calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(75D, result.Value);
        }

        [Fact]
        public async Task Calculate_PromotionBeatsAll_AppliesPromotionDiscount()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Gold);
            var promotion = Promotion.Create(
                "Sommer Rabat",
                40m,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5)
            );

            var calculator = new PriceCalculator(new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new BirthdayPricingStrategy(),
                new PromotionPricingStrategy()
            });

            var result = await calculator.BuildStrategies(client, promotion, sessionType);

            Assert.Equal(60D, result.Value);
        }

        [Fact]
        public async Task Calculate_AlwaysPicksLowestPrice()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Silver);
            var promotion = Promotion.Create("Småting", 5m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var calculator = new PriceCalculator(new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new PromotionPricingStrategy()
            });

            var result = await calculator.BuildStrategies(client, promotion, sessionType);

            Assert.Equal(90D, result.Value);
        }

        [Fact]
        public async Task Calculate_PromotionZeroPercent_ThrowsDomainExceptionOnCreation()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.None);

            Assert.Throws<UserInvalidInputException>(() =>
                Promotion.Create("Gratis Ting", 0m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
            );
        }
    }
}