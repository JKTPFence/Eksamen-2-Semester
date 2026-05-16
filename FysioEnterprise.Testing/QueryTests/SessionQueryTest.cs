using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Infrastructure.QueryHandlers;
using FysioEnterprise.Infrastructure.Database;
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
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(clientId, staffId, sessionTypeId, roomId, sessionTimeSlot1, 395, null, new List<Session>(), new List<Session>(), new List<Session>());

            var cancelledSession = Session.Create(clientId, staffId, sessionTypeId, roomId, sessionTimeSlot2, 395, null, new List<Session>(), new List<Session>(), new List<Session>());
            cancelledSession.CancelSession();

            context.Sessions.AddRange(activeSession, cancelledSession);
            context.SaveChanges();

            var query = new SessionQueriesImpl(context);

            //Act
            var result = await query.GetAllActiveSessionsByClientIdAsync(clientId);

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
            var clientId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var sessionTypeId = Guid.NewGuid();
            var roomId = Guid.NewGuid();

            var sessionTimeSlot1 = new TimeSlot(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));
            var sessionTimeSlot2 = new TimeSlot(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1));

            var activeSession = Session.Create(clientId, staffId, sessionTypeId, roomId, sessionTimeSlot1, 395, null, new List<Session>(), new List<Session>(), new List<Session>());
            var completedSession = Session.Create(clientId, staffId, sessionTypeId, roomId, sessionTimeSlot2, 395, null, new List<Session>(), new List<Session>(), new List<Session>());
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
    }
}
