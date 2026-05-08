using FysioEnterprise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Infrastructure.Database
{
    public class SeedData
    {
        public static class SessionTypeSeed
        {
            public static IEnumerable<SessionType> GetSeedData() => new List<SessionType>
            {
                // Fysioterapi
                new SessionType("Fysioterapi 30 min.",  395, 1, new TimeOnly(0, 30)),
                new SessionType("Fysioterapi 45 min.",  589, 1, new TimeOnly(0, 45)),
                new SessionType("Fysioterapi 60 min.",  745, 1, new TimeOnly(1, 0)),

                // Sportsmassage
                new SessionType("Sportsmassage 30 min.", 350, 1, new TimeOnly(0, 30)),
                new SessionType("Sportsmassage 60 min.", 699, 1, new TimeOnly(1, 0)),

                // Akupunktur
                new SessionType("Akupunktur 45 min.", 550, 1, new TimeOnly(0, 45)),

                // Kostvejledning
                new SessionType("Kostvejledning førstegang",  799, 1, new TimeOnly(1, 0)),
                new SessionType("Kostvejledning opfølgning",  450, 1, new TimeOnly(0, 30)),

                // Holdtræning
                new SessionType("Holdtræning/genoptræning 60 min.", 150, 6, new TimeOnly(1, 0)),
            };
        }
    }
}
