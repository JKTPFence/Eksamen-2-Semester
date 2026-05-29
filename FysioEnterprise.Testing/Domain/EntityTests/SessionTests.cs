using System.Collections;
using System.Net;
using System.Numerics;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;
using Xunit;

namespace FysioEnterprise.Testing.Domain.EntityTests
{
        public class SessionTests
        {
            private static Session BuildSession(
                DateTime? start = null,
                DateTime? end = null)
            {
                var s = start ?? DateTime.UtcNow.AddDays(50);
                var e = end ?? s.AddHours(2);
            var dummyPriceCalculator = new SeedPricingFactory();
            var sessionType = new SessionType(
            "Standard Session",
            new Price (100),
            4,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
            new List<int>()
            );
            var client = Client.Create(
                "Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: LoyaltyLevel.Gold

            );
            var staff = new Staff(
                "Johan",
                "Jensen",
                "johanne@example.com",
                "Akupunktør",
                333333,
                new List<Clinic>()
                );
                var timeSlot = new TimeSlot(s, e);

                return Session.Create(
                    client, staff, sessionType, Guid.NewGuid(),
                    timeSlot,
                    promotion: null, [], [], [], dummyPriceCalculator, new List<OpeningHours>());
                }

            [Fact]
            public void Create_ValidInputs_ReturnsActiveSession()
            {
                var session = BuildSession();

                Assert.Equal(SessionStatusEnum.Active, session.SessionStatus);
                Assert.True(session.IsActive);
                Assert.NotEqual(Guid.Empty, session.Id);
            }

            [Fact]
            public void Create_EmptyRoomId_ThrowsArgumentException()
            {
            var dummyPriceCalculator = new SeedPricingFactory();
            var sessionType = new SessionType(
            "Standard Session",
            new Price(100),
            4,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
            new List<int>()
            );
            var client = Client.Create(
                "Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: LoyaltyLevel.Gold
            );
            var staff = new Staff(
               "Johan",
               "Jensen",
               "johanne@example.com",
               "Akupunktør",
               333333,
               new List<Clinic>()
               );
            var emptyRoomId = Guid.Empty;

            var ex = Assert.Throws<UserInvalidInputException>(() => Session.Create(
                    client, staff, sessionType, emptyRoomId,
                    new TimeSlot(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)),
                    null, [], [], [], dummyPriceCalculator, new List<OpeningHours>()));

            Assert.Equal("For at oprette en booking skal der vælges et rum", ex.Message);
        }


            [Fact]
            public void Create_StartInPast_ThrowsDomainException()
            {
            var dummyPriceCalculator = new SeedPricingFactory();
            var sessionType = new SessionType(
            "Standard Session",
            new Price(100),
            4,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
            new List<int>()
            );
            var client = Client.Create(
                "Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: LoyaltyLevel.Gold
            );
            var staff = new Staff(
             "Johan",
             "Jensen",
             "johanne@example.com",
             "Akupunktør",
             333333,
             new List<Clinic>()
             );

            Assert.Throws<ValidationException>(() => Session.Create(
                    client, staff, sessionType, Guid.NewGuid(),
                    new TimeSlot(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddHours(1)),
                    null, [], [], [], dummyPriceCalculator, new List<OpeningHours>()));
            }

            //Overlap validation tests
            [Fact]
            public void Create_ClientOverlap_ThrowsDomainException()
            {
                var start = DateTime.UtcNow.AddDays(50);
                var end = start.AddHours(1);
                var timeSlot = new TimeSlot(start, end);
                var existing = BuildSession(start, end);
                var dummyPriceCalculator = new SeedPricingFactory();
                var sessionType = new SessionType(
                "Standard Session",
                new Price(100),
                4,
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
                new List<int>()
                );
                var client = Client.Create(
                    "Johanne",
                    "Jensen",
                    "johanne@example.com",
                    "71362851",
                    new DateOnly(1995, 5, 15),
                    "Valløesgade 37, 2. th, 7100 Vejle",
                    clientNote: null,
                    clientPrefferedStaffID: Guid.NewGuid(),
                    clientLoyaltyLevel: LoyaltyLevel.Gold
                );
                var staff = new Staff(
                 "Johan",
                 "Jensen",
                 "johanne@example.com",
                 "Akupunktør",
                 333333,
                 new List<Clinic>()
                 );

            Assert.Throws<ValidationException>(() => Session.Create(
                    client, staff, sessionType, Guid.NewGuid(),
                    timeSlot, null, [existing], [], [], dummyPriceCalculator, new List<OpeningHours>()));
            }

            [Fact]
            public void Create_StaffOverlap_ThrowsDomainException()
            {
                var start = DateTime.UtcNow.AddDays(50);
                var end = start.AddHours(1);
                var timeSlot = new TimeSlot(start, end);
                var existing = BuildSession(start, end);
                var dummyPriceCalculator = new SeedPricingFactory();
                var sessionType = new SessionType(
                "Standard Session",
                new Price(100),
                4,
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)),
                new List<int>()
                );
                var client = Client.Create(
                    "Johanne",
                    "Jensen",
                    "johanne@example.com",
                    "71362851",
                    new DateOnly(1995, 5, 15),
                    "Valløesgade 37, 2. th, 7100 Vejle",
                    clientNote: null,
                    clientPrefferedStaffID: Guid.NewGuid(),
                    clientLoyaltyLevel: LoyaltyLevel.Gold
                );
            var staff = new Staff(
                 "Johan",
                 "Jensen",
                 "johanne@example.com",
                 "Akupunktør",
                 333333,
                 new List<Clinic>()
                 );

            Assert.Throws<ValidationException>(() => Session.Create(
                    client, staff, sessionType, Guid.NewGuid(),
                    timeSlot, null, [], [existing], [existing], dummyPriceCalculator, new List<OpeningHours>()));
            }

            //Update Session Validation tests
            [Fact]
            public void UpdateSessionTime_ValidTimes_UpdatesSession()
            {
                var session = BuildSession();
                var newStart = DateTime.UtcNow.AddHours(3);
                var newEnd = newStart.AddHours(1);
                var timeSlot = new TimeSlot(newStart, newEnd);

            session.UpdateSessionTime(session.Id, timeSlot, [], [], [], new List<OpeningHours>());

                Assert.Equal(newStart, session.SessionTimeSlot.From);
                Assert.Equal(newEnd, session.SessionTimeSlot.To);
            }

            //Completed session tests

            [Fact]
            public void CompletedSession_ActiveSession_SetsCompleted()
            {
                var session = BuildSession();

                session.CompletedSession();

                Assert.Equal(SessionStatusEnum.Completed, session.SessionStatus);
            }

            [Fact]
            public void CompletedSession_InactiveSession_ThrowsInvalidOperationException()
            {
                var session = BuildSession();
                session.CancelSession();

                Assert.Throws<UserInvalidInputException>(() => session.CompletedSession());
            }


            //Cancelled session tests:
            [Fact]
            public void CancelSession_ActiveSession_SetsCancelled()
            {
                var session = BuildSession();

                session.CancelSession();

                Assert.Equal(SessionStatusEnum.Cancelled, session.SessionStatus);
            }

            [Fact]
            public void CancelSession_InactiveSession_ThrowsInvalidOperationException()
            {
                var session = BuildSession();
                session.CompletedSession();

                Assert.Throws<UserInvalidInputException>(() => session.CancelSession());
            }

            [Fact]
            public void NoShowSession_ActiveSession_SetsNoShow()
            {
                var session = BuildSession();

                session.SetNoShowSession();

                Assert.Equal(SessionStatusEnum.NoShow, session.SessionStatus);
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
