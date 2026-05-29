using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Infrastructure.Database;
using FysioEnterprise.Infrastructure.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FysioEnterprise.Testing.QueryHandlerTests
{
    public class SessionQueryTests
    {
        private AppDBContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDBContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private static SessionType CreateMockSessionType(Guid? sessionTypeId = null)
        {
            var sessionType = (SessionType)Activator.CreateInstance(typeof(SessionType), nonPublic: true)!;

            typeof(SessionType).GetProperty("Id")?
                .SetValue(sessionType, sessionTypeId ?? Guid.NewGuid());

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeName))!
                .SetValue(sessionType, "Standard Session");

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypePrice))!
                .SetValue(sessionType, new Price(500));

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeTimeSpan))!
                .SetValue(sessionType, TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)));

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeMaxAmount))!
                .SetValue(sessionType, 4);

            typeof(SessionType).GetProperty(nameof(SessionType.AllowedAuthorisationNumbers))!
                .SetValue(sessionType, new List<int>());

            return sessionType;
        }

        [Fact]
        public async Task GetAllActiveSessionsByClientIdAsync_ReturnsOnlyActiveSessions()
        {
            //Arrange
            var context = GetInMemoryContext();
            var client = Client.Create("Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: LoyaltyLevel.Gold);
                var staff = new Staff(
                        "Johan",
                        "Jensen",
                        "71362851",
                        "Akupunktør",
                        333333,
                        new List<Clinic>());

            typeof(Staff).GetProperty("Id")?
                .SetValue(staff, Guid.NewGuid());

            var dummyPricingFactory = new SeedPricingFactory();
            var sessionType = CreateMockSessionType();
            var roomId = Guid.NewGuid();
            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(client, staff, sessionType, roomId, sessionTimeSlot1, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, new List<OpeningHours>());

            var cancelledSession = Session.Create(client, staff, sessionType, roomId, sessionTimeSlot2, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, new List<OpeningHours>());
            cancelledSession.CancelSession();

            context.Sessions.AddRange(activeSession, cancelledSession);
            context.SaveChanges();

            var query = new SessionQueriesImpl(context);

            //Act
            var result = await query.GetAllActiveSessionsByClientIdAsync(client.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, s => Assert.Equal("Active", s.SessionStatus));
        }

        [Fact]
        public async Task GetAllActiveSessionsByStaffIdAsync_ReturnsOnlyActiveSessions()
        {
            //Arrange
            var context = GetInMemoryContext();
            var client = Client.Create("Johanne",
                "Jensen",
                "johanne@example.com",
                "71362851",
                new DateOnly(1995, 5, 15),
                "Valløesgade 37, 2. th, 7100 Vejle",
                clientNote: null,
                clientPrefferedStaffID: Guid.NewGuid(),
                clientLoyaltyLevel: LoyaltyLevel.Gold);
            var staff = new Staff(
                    "Johan",
                    "Jensen",
                    "71362851",
                    "Akupunktør",
                    333333,
                    new List<Clinic>());

            typeof(Staff).GetProperty("Id")?
                .SetValue(staff, Guid.NewGuid());

            var dummyPricingFactory = new SeedPricingFactory();
            var sessionType = CreateMockSessionType();
            var roomId = Guid.NewGuid();

            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(client, staff, sessionType, roomId, sessionTimeSlot1, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, new List<OpeningHours>());
            var completedSession = Session.Create(client, staff, sessionType, roomId, sessionTimeSlot2, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory, new List<OpeningHours>());
            completedSession.CompletedSession();

            context.Sessions.AddRange(activeSession, completedSession);
            context.SaveChanges();

            var query = new SessionQueriesImpl(context);

            //Act
            var result = await query.GetAllActiveSessionsByStaffIdAsync(staff.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, s => Assert.Equal("Active", s.SessionStatus));
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
