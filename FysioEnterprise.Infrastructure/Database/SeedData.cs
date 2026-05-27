using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace FysioEnterprise.Infrastructure.Database
{
    public class SeedData
    {
        public static class SessionTypeSeed
        {
            public static List<SessionType> GetSeedData()
            {
                var sessiontype1 = new SessionType("Fysioterapi", new Price(395), 1, new TimeOnly(0, 30), new List<int> {44444});
                var sessiontype2 = new SessionType("Fysioterapi", new Price(589), 1, new TimeOnly(0, 45), new List<int> {44444});
                var sessiontype3 = new SessionType("Fysioterapi", new Price(745), 1, new TimeOnly(1, 0), new List<int> {44444});
                var sessiontype4 = new SessionType("Sportsmassage", new Price(350), 1, new TimeOnly(0, 30), new List<int> { 44444, 33333 });
                var sessiontype5 = new SessionType("Sportsmassage", new Price(699), 1, new TimeOnly(1, 0), new List<int> { 44444, 33333 });
                var sessiontype6 = new SessionType("Akupunktur", new Price(550), 1, new TimeOnly(0, 45), new List<int> { 11111 });
                var sessiontype7 = new SessionType("Kostvejledning førstegang", new Price(799), 1, new TimeOnly(1, 0), new List<int> { 55555 });
                var sessiontype8 = new SessionType("Kostvejledning opfølgning", new Price(450), 1, new TimeOnly(0, 30), new List<int> { 55555 });
                var sessiontype9 = new SessionType("Genoptræning", new Price(150), 6, new TimeOnly(1, 0), new List<int> { 44444, 66666});

                return new List<SessionType> { sessiontype1, sessiontype2, sessiontype3, sessiontype4, sessiontype5, sessiontype6, sessiontype7, sessiontype8, sessiontype9 };
            }
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
                var staff1 = new Staff("Anders", "Nielsen", "anders@bookright.dk", "Fysioterapeut", 44444, new List<Clinic> {});
                var staff2 = new Staff("Maria", "Hansen", "maria@bookright.dk", "Massør", 33333, new List<Clinic> {});
                var staff3 = new Staff("Lars", "Pedersen", "lars@fysio.dk", "Akupunktør", 11111, new List<Clinic> {});
                var staff4 = new Staff("Sofie", "Jensen", "jense@bookright.dk", "Kostvejleder", 55555, new List<Clinic> {});
                var staff5 = new Staff("Karl", "Lennart", "KarlL@bookright.dk", "Fysioterapeut", 44444, new List<Clinic> { });
                var staff6 = new Staff("Mette", "Larsen", "metteJHL@bookright.dk", "Træner", 66666, new List<Clinic> { });
                var staff7 = new Staff("Thomas", "Christensen", "thomas@bookright.dk", "Fysioterapeut", 44444, new List<Clinic> { });
                var staff8 = new Staff("Camilla", "Møller", "camilla@bookright.dk", "Massør", 33333, new List<Clinic> { });
                var staff9 = new Staff("Rasmus", "Andersen", "rasmus@bookright.dk", "Akupunktør", 11111, new List<Clinic> { });
                var staff10 = new Staff("Louise", "Thomsen", "louise@bookright.dk", "Kostvejleder", 55555, new List<Clinic> { });
                var staff11 = new Staff("Mikkel", "Kristensen", "mikkel@bookright.dk", "Træner", 66666, new List<Clinic> { });
                var staff12 = new Staff("Stine", "Rasmussen", "stine@bookright.dk", "Fysioterapeut", 44444, new List<Clinic> { });

                var receptionist1 = new Staff("Sofie", "Jørgensen", "smj01@bookright.dk", "Receptionist", 22222, new List<Clinic> {});
                var receptionist2 = new Staff("Emilie", "Hansen", "erh97@bookright.dk", "Receptionist", 22222, new List<Clinic> { });
                var receptionist3 = new Staff("Jan", "Krabbe", "jk85@bookright.dk", "Receptionist", 22222, new List<Clinic> { });
                var receptionist4 = new Staff("Lone", "Madsen", "lm54@bookright.dk", "Receptionist", 22222, new List<Clinic> { });
                var receptionist5 = new Staff("Peter", "Andersen", "pa98@bookright.dk", "Receptionist", 22222, new List<Clinic> { });

                staff1.AssignToClinic(clinics[0].Id);
                staff1.AssignToClinic(clinics[1].Id);
                staff2.AssignToClinic(clinics[0].Id);
                staff2.AssignToClinic(clinics[1].Id);
                staff3.AssignToClinic(clinics[1].Id);
                staff3.AssignToClinic(clinics[0].Id);
                staff4.AssignToClinic(clinics[2].Id);
                staff5.AssignToClinic(clinics[2].Id);
                staff6.AssignToClinic(clinics[1].Id);
                staff6.AssignToClinic(clinics[2].Id);
                staff7.AssignToClinic(clinics[0].Id);
                staff8.AssignToClinic(clinics[0].Id);
                staff8.AssignToClinic(clinics[2].Id);
                staff9.AssignToClinic(clinics[1].Id);
                staff10.AssignToClinic(clinics[2].Id);
                staff10.AssignToClinic(clinics[0].Id);
                staff11.AssignToClinic(clinics[0].Id);
                staff11.AssignToClinic(clinics[1].Id);
                staff11.AssignToClinic(clinics[2].Id);
                staff12.AssignToClinic(clinics[2].Id);

                receptionist1.AssignToClinic(clinics[0].Id);
                receptionist1.AssignToClinic(clinics[1].Id);
                receptionist2.AssignToClinic(clinics[1].Id);
                receptionist3.AssignToClinic(clinics[2].Id);
                receptionist4.AssignToClinic(clinics[2].Id);
                receptionist5.AssignToClinic(clinics[1].Id);
                receptionist5.AssignToClinic(clinics[0].Id);


                return new List<Staff> { staff1, staff2, staff3, staff4, staff5, staff6, staff7, staff8, staff9, staff10, staff11, staff12, receptionist1, receptionist2, receptionist3, receptionist4, receptionist5};
            }
        }

        public static class ClientSeed
        {
            public static List<Client> GetSeedData(List<Staff> staff)
            {
                var client1 = Client.Create("Hans", "Jensen", "hans@gmail.com", "+45 12 34 56 78", new DateOnly(1985, 3, 15), "Vejlevej 1, 7100 Vejle", "Problemer med banan", staff[0].Id, LoyaltyLevel.Gold);
                var client2 = Client.Create("Anna", "Sørensen", "anna@gmail.com", "+45 87 65 43 21", new DateOnly(1990, 7, 22), "Østergade 1, 6040 Egtved", "Allergi over for latex", staff[1].Id, LoyaltyLevel.Silver);
                var client3 = Client.Create("Peter", "Lystskov", "peter@gmail.com", "+45 11 22 33 44", new DateOnly(1978, 11, 5), "Nørregade 3, 7100 Vejle", "Allergi over for perfurme - brug neutral", staff[2].Id, LoyaltyLevel.None);
                var client4 = Client.Create("Mette", "Christensen", "mette@gmail.com", "+45 23 45 67 89", new DateOnly(1992, 5, 8), "Søndergade 12, 7100 Vejle", "Tidligere knæskade - venstre knæ", staff[0].Id, LoyaltyLevel.Bronze);
                var client5 = Client.Create("Søren", "Madsen", "soren@gmail.com", "+45 34 56 78 90", new DateOnly(1975, 9, 14), "Kongevej 7, 6040 Egtved", null, staff[1].Id, LoyaltyLevel.None);
                var client6 = Client.Create("Camilla", "Møller", "camilla@gmail.com", "+45 45 67 89 01", new DateOnly(1988, 2, 28), "Kirkegade 3, 7100 Vejle", "Claustrofobisk - undgå lukkede rum", staff[2].Id, LoyaltyLevel.Silver);
                var client7 = Client.Create("Thomas", "Nielsen", "thomas@gmail.com", "+45 56 78 90 12", new DateOnly(1983, 12, 3), "Vestergade 22, 6040 Egtved", "Diabetes type 2", staff[0].Id, LoyaltyLevel.Gold);
                var client8 = Client.Create("Louise", "Hansen", "louise@gmail.com", "+45 67 89 01 23", new DateOnly(1995, 4, 17), "Bakkevej 9, 7100 Vejle", null, staff[1].Id, LoyaltyLevel.None);
                var client9 = Client.Create("Rasmus", "Pedersen", "rasmus@gmail.com", "+45 78 90 12 34", new DateOnly(1970, 8, 25), "Havnevej 5, 6040 Egtved", "Høreapparat - tal tydeligt", staff[2].Id, LoyaltyLevel.Bronze);
                var client10 = Client.Create("Sofie", "Andersen", "sofie@gmail.com", "+45 89 01 23 45", new DateOnly(2000, 1, 11), "Rosenvej 14, 7100 Vejle", "Gravid - undgå maveliggende øvelser", staff[0].Id, LoyaltyLevel.None);

                return new List<Client> { client1, client2, client3, client4, client5, client6, client7, client8, client9, client10 };
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
            public static List<Session> GetSeedData(List<Client> clients, List<Staff> staff, List<SessionType> sessionTypes, List<Clinic> clinics, List<Promotion> promotions)
            {
                var dummyPricingFactory = new SeedPricingFactory();

                DateTime today = DateTime.UtcNow.Date;
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                if (daysUntilMonday == 0) daysUntilMonday = 7;

                DateTime nextMonday = today.AddDays(daysUntilMonday);
                DateTime nextTuesday = nextMonday.AddDays(1);
                DateTime nextWednesday = nextMonday.AddDays(2);
                DateTime nextThursday = nextMonday.AddDays(3);
                DateTime nextFriday = nextMonday.AddDays(4);

                var session1 = Session.Create(clients[0], staff[0], sessionTypes[0], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextTuesday.AddHours(11).AddMinutes(0), nextTuesday.AddHours(12).AddMinutes(0)), promotions[1], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session2 = Session.Create(clients[1], staff[1], sessionTypes[4], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextMonday.AddHours(11).AddMinutes(15), nextMonday.AddHours(12).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session3 = Session.Create(clients[2], staff[2], sessionTypes[5], clinics[1].ClinicRooms[0].Id, new TimeSlot(nextWednesday.AddHours(12).AddMinutes(0), nextWednesday.AddHours(13).AddMinutes(0)), promotions[0], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session4 = Session.Create(clients[2], staff[0], sessionTypes[0], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextMonday.AddHours(13).AddMinutes(0), nextMonday.AddHours(14).AddMinutes(0)), promotions[0], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session5 = Session.Create(clients[1], staff[1], sessionTypes[3], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextMonday.AddHours(13).AddMinutes(15), nextMonday.AddHours(14).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session6 = Session.Create(clients[0], staff[2], sessionTypes[4], clinics[1].ClinicRooms[1].Id, new TimeSlot(nextTuesday.AddHours(14).AddMinutes(0), nextTuesday.AddHours(15).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session7 = Session.Create(clients[1], staff[0], sessionTypes[1], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextTuesday.AddHours(13), nextTuesday.AddHours(14)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session8 = Session.Create(clients[2], staff[1], sessionTypes[2], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextWednesday.AddHours(8).AddMinutes(0), nextWednesday.AddHours(9).AddMinutes(0)), promotions[0], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session9 = Session.Create(clients[1], staff[1], sessionTypes[0], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextWednesday.AddHours(9).AddMinutes(0), nextWednesday.AddHours(9).AddMinutes(45)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session10 = Session.Create(clients[2], staff[0], sessionTypes[5], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextWednesday.AddHours(11).AddMinutes(0), nextWednesday.AddHours(12).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session11 = Session.Create(clients[0], staff[0], sessionTypes[1], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextWednesday.AddHours(11).AddMinutes(30), nextWednesday.AddHours(12).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session12 = Session.Create(clients[1], staff[2], sessionTypes[3], clinics[1].ClinicRooms[0].Id, new TimeSlot(nextThursday.AddHours(13).AddMinutes(0), nextThursday.AddHours(14).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session13 = Session.Create(clients[2], staff[0], sessionTypes[4], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextThursday.AddHours(14).AddMinutes(0), nextThursday.AddHours(14).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session14 = Session.Create(clients[2], staff[1], sessionTypes[2], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextThursday.AddHours(14), nextThursday.AddHours(15)), promotions[0], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session15 = Session.Create(clients[0], staff[2], sessionTypes[0], clinics[1].ClinicRooms[1].Id, new TimeSlot(nextFriday.AddHours(8).AddMinutes(30), nextFriday.AddHours(9).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session16 = Session.Create(clients[0], staff[0], sessionTypes[1], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextFriday.AddHours(11).AddMinutes(0), nextFriday.AddHours(12).AddMinutes(0)), promotions[1], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session17 = Session.Create(clients[1], staff[1], sessionTypes[5], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextFriday.AddHours(11).AddMinutes(15), nextFriday.AddHours(12).AddMinutes(15)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session18 = Session.Create(clients[2], staff[2], sessionTypes[2], clinics[1].ClinicRooms[0].Id, new TimeSlot(nextFriday.AddHours(11).AddMinutes(30), nextFriday.AddHours(12).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session19 = Session.Create(clients[2], staff[0], sessionTypes[3], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextFriday.AddHours(12).AddMinutes(30), nextFriday.AddHours(13).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session20 = Session.Create(clients[1], staff[1], sessionTypes[4], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextFriday.AddHours(13).AddMinutes(0), nextFriday.AddHours(14).AddMinutes(0)), promotions[1], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session21 = Session.Create(clients[0], staff[0], sessionTypes[0], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextMonday.AddHours(8).AddMinutes(0), nextMonday.AddHours(8).AddMinutes(30)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session22 = Session.Create(clients[1], staff[1], sessionTypes[1], clinics[0].ClinicRooms[1].Id, new TimeSlot(nextMonday.AddHours(9).AddMinutes(0), nextMonday.AddHours(10).AddMinutes(0)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);
                var session23 = Session.Create(clients[2], staff[2], sessionTypes[2], clinics[1].ClinicRooms[0].Id, new TimeSlot(nextMonday.AddHours(8), nextMonday.AddHours(8).AddMinutes(45)), null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[1].ClinicOpeningHours);
                var session24 = Session.Create(clients[2], staff[1], sessionTypes[2], clinics[0].ClinicRooms[0].Id, new TimeSlot(nextWednesday.AddHours(9).AddMinutes(0), nextWednesday.AddHours(10).AddMinutes(0)), promotions[0], new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, clinics[0].ClinicOpeningHours);


                return new List<Session> { session1,  session2, session3, session4, session5, session6, session7, session8, session9, session10, session11, session12, session13, session14, session15, session16, session17, session18, session19, session20, session21, session22, session23};
            }
        }
        private class SeedPricingFactory : IPricingStrategyFactory
        {
            public Task<Price> BuildStrategies(Client client, Promotion? promotion, SessionType sessionType)
            {
                // Fallback default value object returned to bypass domain creation invariants smoothly
                return Task.FromResult(new Price(450));
            }
        }
    }
}
