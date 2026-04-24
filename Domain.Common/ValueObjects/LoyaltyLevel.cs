using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class LoyaltyLevel
    {
        public string LoyaltyLevelName { get; private set; }
        public int LoyaltyLevelDiscountPercent { get; private set; }

        public LoyaltyLevel(string loyaltyLevelName, int loyaltyLevelDiscountPercent)
        {
            LoyaltyLevelName = loyaltyLevelName;
            LoyaltyLevelDiscountPercent = loyaltyLevelDiscountPercent;
        }
    }
}
