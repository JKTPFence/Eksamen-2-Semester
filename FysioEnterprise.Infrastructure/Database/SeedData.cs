using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.ValueObjects;

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

        public static class ClinicSeed
        {
            public static List<Clinic> GetSeedData()
            {
                var clinic1 = new Clinic("Vejle Klinik, Boulevarden 24, 7100 Vejle",
                    new List<OpeningHours>
                    {
                        new(DayOfWeek.Monday,    new TimeOnly(8, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Tuesday,   new TimeOnly(8, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Wednesday, new TimeOnly(8, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Thursday,  new TimeOnly(8, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Friday,    new TimeOnly(8, 0),  new TimeOnly(14, 0)),
                        OpeningHours.Closed(DayOfWeek.Saturday),
                        OpeningHours.Closed(DayOfWeek.Sunday),
                    }, 
                    new List<Room>());
                
                var clinic2 = new Clinic("Egtved Klinik, Østergade 5, 6040 Egtved", 
                    new List<OpeningHours>
                    {
                        new(DayOfWeek.Monday,    new TimeOnly(7, 0),  new TimeOnly(15, 0)),
                        new(DayOfWeek.Tuesday,   new TimeOnly(7, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Wednesday, new TimeOnly(7, 0),  new TimeOnly(15, 30)),
                        new(DayOfWeek.Thursday,  new TimeOnly(7, 0),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Friday,    new TimeOnly(7, 0),  new TimeOnly(14, 0)),
                        OpeningHours.Closed(DayOfWeek.Saturday),
                        OpeningHours.Closed(DayOfWeek.Sunday),
                    }, new List<Room>());

                var clinic3 = new Clinic("Hedensted Klinik, Hovedvejen 9, 8722 Hedensted",
                    new List<OpeningHours>
                    {
                        new(DayOfWeek.Monday,    new TimeOnly(7, 30),  new TimeOnly(15, 0)),
                        new(DayOfWeek.Tuesday,   new TimeOnly(7, 30),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Wednesday, new TimeOnly(7, 30),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Thursday,  new TimeOnly(7, 30),  new TimeOnly(16, 0)),
                        new(DayOfWeek.Friday,    new TimeOnly(7, 30),  new TimeOnly(14, 0)),
                        OpeningHours.Closed(DayOfWeek.Saturday),
                        OpeningHours.Closed(DayOfWeek.Sunday),
                    }, new List<Room>());

                //Oprettelse af rum
                var room1 = new Room(clinic1, 01);
                var room2 = new Room(clinic1, 02);
                var room3 = new Room(clinic2, 01);
                var room4 = new Room(clinic2, 02);
                var room5 = new Room(clinic3, 01);
                var room6 = new Room(clinic3, 02);
                var room7 = new Room(clinic3, 03);

                //Tilføjelse af rum til klinikker
                clinic1 = new Clinic(clinic1.ClinicAddress, clinic1.ClinicOpeningHours, new List<Room> { room1, room2 });
                clinic2 = new Clinic(clinic2.ClinicAddress, clinic2.ClinicOpeningHours, new List<Room> { room3, room4 });
                clinic3 = new Clinic(clinic3.ClinicAddress, clinic3.ClinicOpeningHours, new List<Room> { room5, room6, room7 });

                return new List<Clinic> { clinic1, clinic2, clinic3 };
            }
        }

        public static class StaffSeed
        {
            public static List<Staff> GetSeedData(List<Clinic> clinics)
            {
                var staff1 = new Staff("Anders", "Nielsen", "anders@bookright.dk", "Fysioterapeut", 444444, new List<Clinic> {});
                var staff2 = new Staff("Maria", "Hansen", "maria@bookright.dk", "Massør", 33333, new List<Clinic> {});
                var staff3 = new Staff("Lars", "Pedersen", "lars@fysio.dk", "Akupunktør", 11111, new List<Clinic> {});
                var staff4 = new Staff("Sofie", "Jensen", "jense@bookright.dk", "Kostvejleder", 55555, new List<Clinic> {});
                var staff5 = new Staff("Karl", "Lennart", "KarlL@bookright.dk", "Fysioterapeut", 444444, new List<Clinic> { });

                var receptionist1 = new Staff("Sofie", "Jørgensen", "smj01@bookwell.dk", "Receptionist", 22222, new List<Clinic> {});
                var receptionist2 = new Staff("Emilie", "Hansen", "erh97@bookwell.dk", "Receptionist", 22222, new List<Clinic> { });
                var receptionist3 = new Staff("Jan", "Krabbe", "jk85@bookwell.dk", "Receptionist", 22222, new List<Clinic> { });

                staff1.AssignToClinic(clinics[0].Id);
                staff2.AssignToClinic(clinics[0].Id);
                staff2.AssignToClinic(clinics[1].Id);
                staff3.AssignToClinic(clinics[1].Id);
                staff4.AssignToClinic(clinics[2].Id);
                staff5.AssignToClinic(clinics[2].Id);

                receptionist1.AssignToClinic(clinics[0].Id);
                receptionist2.AssignToClinic(clinics[1].Id);
                receptionist3.AssignToClinic(clinics[2].Id);


                return new List<Staff> { staff1, staff2, staff3, staff4, staff5, receptionist1, receptionist2, receptionist3};
            }
        }

        public static class ClientSeed
        {
            public static List<Client> GetSeedData(List<Staff> staff)
            {
                var client1 = Client.Create("Hans", "Jensen", "hans@gmail.com", "+45 12 34 56 78", new DateOnly(1985, 3, 15), "Vejlevej 1, 7100 Vejle", null, staff[0].Id, LoyaltyLevel.Gold);
                var client2 = Client.Create("Anna", "Sørensen", "anna@gmail.com", "+45 87 65 43 21", new DateOnly(1990, 7, 22), "Østergade 1, 6040 Egtved", "Allergi over for latex", staff[1].Id, LoyaltyLevel.Silver);
                var client3 = Client.Create("Peter", "Lystskov", "peter@gmail.com", "+45 11 22 33 44", new DateOnly(1978, 11, 5), "Nørregade 3, 7100 Vejle", null, staff[2].Id, LoyaltyLevel.None);

                return new List<Client> { client1, client2, client3 };
            }
        }

        public static class PromotionSeed
        {
            public static List<Promotion> GetSeedData()
            {
                var promotion1 = Promotion.Create("Sommerkampagne", 15, new DateTime(2026, 6, 1), new DateTime(2026, 8, 31));
                var promotion2 = Promotion.Create("Forårskampagne", 20, new DateTime(2026, 3, 1), new DateTime(2026, 5, 31));

                return new List<Promotion> { promotion1, promotion2 };
            }
        }

        public class SessionSeed
        {
            public static List<Session> GetSeedData(List<Client> clients, List<Staff> staff, List<SessionType> sessionTypes, List<Clinic> clinics)
            {
                var session1 = Session.Create(clients[0].Id, staff[0].Id, sessionTypes[0].Id, clinics[0].ClinicRooms[0].Id, new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddMinutes(30)), 395, null, new List<Session>(), new List<Session>(), new List<Session>());
                var session2 = Session.Create(clients[1].Id, staff[1].Id, sessionTypes[4].Id, clinics[0].ClinicRooms[1].Id, new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1)), 699, null, new List<Session>(), new List<Session>(), new List<Session>());
                var session3 = Session.Create(clients[2].Id, staff[2].Id, sessionTypes[5].Id, clinics[1].ClinicRooms[0].Id, new TimeSlot(DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(3).AddMinutes(45)), 550, null, new List<Session>(), new List<Session>(), new List<Session>());

                return new List<Session> { session1,  session2, session3 };
            }
        }
    }
}
