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

        [Fact]
        public async Task GetAllActiveSessionsByClientIdAsync_ReturnsOnlyActiveSessions()
        {
            //Arrange
            var context = GetInMemoryContext();
            var client = new Client();
            var staffId = Guid.NewGuid();
            var dummyPricingFactory = new SeedPricingFactory();
            var sessionType = (SessionType)Activator.CreateInstance(
             typeof(SessionType),
             nonPublic: true)!;

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeName))!
                .SetValue(sessionType, "Test Type");
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypePrice))!
                .SetValue(sessionType, new Price(500));
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeTimeSpan))!
                .SetValue(sessionType, TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)));
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeMaxAmount))!
                .SetValue(sessionType, 4);
            typeof(SessionType).GetProperty(nameof(SessionType.AllowedAuthorisationNumbers))!
                .SetValue(sessionType, new List<int>());
            var roomId = Guid.NewGuid();
            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(client, staffId, sessionType, roomId, sessionTimeSlot1, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory);

            var cancelledSession = Session.Create(client, staffId, sessionType, roomId, sessionTimeSlot2, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory);
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
            var client = new Client();
            var dummyPricingFactory = new SeedPricingFactory();
            var sessionType = (SessionType)Activator.CreateInstance(
             typeof(SessionType),
             nonPublic: true)!;

            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeName))!
                .SetValue(sessionType, "Test Type");
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypePrice))!
                .SetValue(sessionType, new Price(500));
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeTimeSpan))!
                .SetValue(sessionType, TimeOnly.FromTimeSpan(TimeSpan.FromHours(1)));
            typeof(SessionType).GetProperty(nameof(SessionType.SessionTypeMaxAmount))!
                .SetValue(sessionType, 4);
            typeof(SessionType).GetProperty(nameof(SessionType.AllowedAuthorisationNumbers))!
                .SetValue(sessionType, new List<int>());
            var staffId = Guid.NewGuid();
            var roomId = Guid.NewGuid();

            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(client, staffId, sessionType, roomId, sessionTimeSlot1, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory);
            var completedSession = Session.Create(client, staffId, sessionType, roomId, sessionTimeSlot2, null, new List<Session>(), new List<Session>(), new List<Session>(), dummyPricingFactory);
            completedSession.CompletedSession();

            context.Sessions.AddRange(activeSession, completedSession);
            context.SaveChanges();

            var query = new SessionQueriesImpl(context);

            //Act
            var result = await query.GetAllActiveSessionsByStaffIdAsync(staffId);

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
