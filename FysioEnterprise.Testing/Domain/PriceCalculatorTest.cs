using System;
using System.Collections.Generic;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Domain.ValueObjects;
using Xunit;
namespace FysioEnterprise.Domain.Tests
{
    public class PriceCalculatorTests
    {
        private static readonly Price BasePriceValue = new Price(100D);

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
                birthDate ?? new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: loyaltyLevel
            );

            return (client, sessionType);
        }

        [Fact]
        public void Calculate_NoStrategies_ReturnsBasePrice()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.None);
            var strategies = new List<IPricingStrategy>();
            var calculator = new PriceCalculator(strategies);

            var result = calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(BasePriceValue.Value, result.Value);
        }

        [Fact]
        public void Calculate_OnlyLoyalty_AppliesLoyaltyDiscount()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Gold);

            var strategies = new List<IPricingStrategy> { new LoyaltyPricingStrategy() };
            var calculator = new PriceCalculator(strategies);

            var result = calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(85D, result.Value);
        }

        [Fact]
        public void Calculate_BirthdayBeatLoyalty_AppliesBirthdayDiscount()
        {
            var birthday = DateOnly.FromDateTime(DateTime.Today);
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Bronze, birthday);

            var strategies = new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new BirthdayPricingStrategy()
            };
            var calculator = new PriceCalculator(strategies);

            var result = calculator.BuildStrategies(client, null, sessionType);

            Assert.Equal(75D, result.Value);
        }

        [Fact]
        public void Calculate_PromotionBeatsAll_AppliesPromotionDiscount()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Gold);
            var promotion = Promotion.Create(
                "Sommer Rabat",
                40m,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5)
            );

            var strategies = new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new BirthdayPricingStrategy(),
                new PromotionPricingStrategy()
            };
            var calculator = new PriceCalculator(strategies);

            var result = calculator.BuildStrategies(client, promotion, sessionType);

            Assert.Equal(60D, result.Value);
        }

        [Fact]
        public void Calculate_AlwaysPicksLowestPrice()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.Silver);
            var promotion = Promotion.Create("Småting", 5m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var strategies = new List<IPricingStrategy>
            {
                new LoyaltyPricingStrategy(),
                new PromotionPricingStrategy()
            };
            var calculator = new PriceCalculator(strategies);

            var result = calculator.BuildStrategies(client, promotion, sessionType);

            Assert.Equal(90D, result.Value);
        }

        [Fact]
        public void Calculate_PromotionZeroPercent_ThrowsDomainExceptionOnCreation()
        {
            var (client, sessionType) = CreateBaseDomainContext(LoyaltyLevel.None);

            Assert.Throws<DomainException>(() =>
                Promotion.Create("Gratis Ting", 0m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
            );
        }
    }
}