using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class LoyaltyLevel : ValueObject
    {
        public string LoyaltyLevelName { get; }
        public decimal LoyaltyLevelDiscountPercentage { get; }

        public LoyaltyLevel() { } // Empty constructor for EF Core

        private LoyaltyLevel(string loyaltyLevelName, decimal loyaltyLevelDiscountPercentage)
        {
            LoyaltyLevelName = loyaltyLevelName;
            LoyaltyLevelDiscountPercentage = loyaltyLevelDiscountPercentage;
        }

        public static readonly LoyaltyLevel None = new("None", 0m);
        public static readonly LoyaltyLevel Bronze = new("Bronze", 5m);
        public static readonly LoyaltyLevel Silver = new("Silver", 10m);
        public static readonly LoyaltyLevel Gold = new("Gold", 15m);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LoyaltyLevelName;
            yield return LoyaltyLevelDiscountPercentage;
        }
    }
}
