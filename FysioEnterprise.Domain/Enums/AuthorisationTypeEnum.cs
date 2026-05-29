using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Enums
{
    public enum AuthorisationTypeEnum
    {
        Standard = 0, 
        Fysioterapeut = 1,  
        Akupunktuor = 2
    }

    public static class AuthorisationTypeExtensions
    {
        public static AuthorisationTypeEnum FromRoleString(string role) => role?.ToLower().Trim() switch // Lower case AthorisationType to Enum values for easier comparison
        {
            "fysioterapeut" => AuthorisationTypeEnum.Fysioterapeut,
            "akupunktør" => AuthorisationTypeEnum.Akupunktuor,
            _ => AuthorisationTypeEnum.Standard
        };

        public static double GetPriceMultiplier(this AuthorisationTypeEnum authType) => authType switch //returns a price multiplier based on the AuthorisationTypeEnum
        {
            AuthorisationTypeEnum.Fysioterapeut => 1.25D,
            AuthorisationTypeEnum.Akupunktuor => 1.15D, 
            _ => 1D 
        };
    }
}
