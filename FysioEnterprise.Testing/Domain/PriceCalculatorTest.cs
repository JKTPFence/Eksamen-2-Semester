using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.Service.PricingService.Strategies;
using FysioEnterprise.Domain.Service.PricingService.Strategies.PricingMethods;
using FysioEnterprise.Domain.ValueObjects;
using Xunit;

namespace FysioEnterprise.Testing.Domain
{
    public class PriceCalculatorTests
    {
        private const decimal BasePrice = 100m;
        private readonly PriceCalculator _calculator = new PriceCalculator();

        [Fact]
        public void Calculate_NoStrategies_ReturnsBasePrice()
        {
            var result = _calculator.Calculate(BasePrice, new List<IPricingStrategy>());
            Assert.Equal(BasePrice, result);
        }

        [Fact]
        public void Calculate_OnlyLoyalty_AppliesLoyaltyDiscount()
        {
            var strategies = new List<IPricingStrategy>
        {
            new LoyaltyPricingStrategy(LoyaltyLevel.Gold) 
        };

            var result = _calculator.Calculate(BasePrice, strategies);

            Assert.Equal(85m, result);
        }

        [Fact]
        public void Calculate_BirthdayBeatLoyalty_AppliesBirthdayDiscount()
        {
            var strategies = new List<IPricingStrategy>
        {
            new LoyaltyPricingStrategy(LoyaltyLevel.Bronze), 
            new BirthdayPricingStrategy()                    
        };

            var result = _calculator.Calculate(BasePrice, strategies);

            Assert.Equal(75m, result);
        }

        [Fact]
        public void Calculate_PromotionBeatsAll_AppliesPromotionDiscount()
        {
            var strategies = new List<IPricingStrategy>
        {
            new LoyaltyPricingStrategy(LoyaltyLevel.Gold), 
            new BirthdayPricingStrategy(),                  
            new PromotionPricingStrategy(40m)               
        };

            var result = _calculator.Calculate(BasePrice, strategies);

            Assert.Equal(60m, result);
        }

        [Fact]
        public void Calculate_AlwaysPicksLowestPrice()
        {
            var strategies = new List<IPricingStrategy>
        {
            new LoyaltyPricingStrategy(LoyaltyLevel.Silver), 
            new PromotionPricingStrategy(5m)                
        };

            var result = _calculator.Calculate(BasePrice, strategies);

            Assert.Equal(90m, result);
        }

        [Fact]
        public void Calculate_PromotionZeroPercent_ReturnsBasePrice()
        {
            var strategies = new List<IPricingStrategy>
        {
            new PromotionPricingStrategy(0m)
        };

            var result = _calculator.Calculate(BasePrice, strategies);

            Assert.Equal(BasePrice, result);
        }
    }
}
