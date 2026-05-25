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
        public static AuthorisationTypeEnum FromRoleString(string role) => role?.ToLower().Trim() switch
        {
            "fysioterapeut" => AuthorisationTypeEnum.Fysioterapeut,
            "akupunktør" => AuthorisationTypeEnum.Akupunktuor,
            _ => AuthorisationTypeEnum.Standard
        };

        public static double GetPriceMultiplier(this AuthorisationTypeEnum authType) => authType switch
        {
            AuthorisationTypeEnum.Fysioterapeut => 1.25D,
            AuthorisationTypeEnum.Akupunktuor => 1.15D, 
            _ => 1D 
        };
    }
}
