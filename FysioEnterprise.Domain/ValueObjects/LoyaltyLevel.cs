using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class LoyaltyLevel : ValueObject
    {
        public string LoyaltyLevelName { get; private init; }
        public decimal LoyaltyLevelDiscountPercentage { get; private init; }

        public LoyaltyLevel() { } // Empty constructor for EF Core

        private LoyaltyLevel(string loyaltyLevelName, decimal loyaltyLevelDiscountPercentage)
        {
            LoyaltyLevelName = loyaltyLevelName;
            LoyaltyLevelDiscountPercentage = loyaltyLevelDiscountPercentage;
        }

        public static LoyaltyLevel None => new LoyaltyLevel("None", 0m);
        public static LoyaltyLevel Bronze => new LoyaltyLevel("Bronze", 5m);
        public static LoyaltyLevel Silver => new LoyaltyLevel("Silver", 10m);
        public static LoyaltyLevel Gold => new LoyaltyLevel("Gold", 15m);

        public static LoyaltyLevel FromName(string name) => name?.ToLower().Trim() switch
        {
            "Bronze" => Bronze,
            "Silver" => Silver,
            "Gold" => Gold,
            _ => None
        };

        public static LoyaltyLevel CalculateFromSpend(double totalSpendInLast12Months) => totalSpendInLast12Months switch
        {
            > 25000D => Gold,

            >= 10001D => Silver,

            >= 3000D => Bronze,

            _ => None
        };

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LoyaltyLevelName;
            yield return LoyaltyLevelDiscountPercentage;
        }
    }
}
