using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class LoyaltyLevel
    {
        public string LoyaltyLevelName { get; }
        public decimal LoyaltyLevelDiscountPercentage { get; }

        private LoyaltyLevel(string loyaltyLevelName, decimal loyaltyLevelDiscountPercentage)
        {
            LoyaltyLevelName = loyaltyLevelName;
            LoyaltyLevelDiscountPercentage = loyaltyLevelDiscountPercentage;
        }

        public static readonly LoyaltyLevel None = new("None", 0m);
        public static readonly LoyaltyLevel Bronze = new("Bronze", 5m);
        public static readonly LoyaltyLevel Silver = new("Silver", 10m);
        public static readonly LoyaltyLevel Gold = new("Gold", 15m);
    }
}
