using System.Collections;
using FysioEnterprise.Domain.Entities;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
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
                var s = start ?? DateTime.UtcNow.AddHours(1);
                var timeSlot = new TimeSlot(s, end ?? s.AddHours(1));
                return Session.Create(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    timeSlot,
                    totalPrice: 100m, promotionId: null, [], [], []);
            }

            [Fact]
            public void Create_ValidInputs_ReturnsActiveSession()
            {
                var session = BuildSession();

                Assert.Equal(SessionStatusEnum.Active, session.SessionStatus);
                Assert.True(session.IsActive);
                Assert.NotEqual(Guid.Empty, session.Id);
            }

            [Theory]
            [InlineData("clientId")]
            [InlineData("staffId")]
            [InlineData("sessionTypeId")]
            [InlineData("roomId")]
            public void Create_EmptyGuid_ThrowsArgumentNullException(string paramName)
            {
                var ids = new Dictionary<string, Guid>
                {
                    ["clientId"] = Guid.NewGuid(),
                    ["staffId"] = Guid.NewGuid(),
                    ["sessionTypeId"] = Guid.NewGuid(),
                    ["roomId"] = Guid.NewGuid(),
                };
                ids[paramName] = Guid.Empty;

                var ex = Assert.Throws<ArgumentNullException>(() => Session.Create(
                    ids["clientId"], ids["staffId"], ids["sessionTypeId"], ids["roomId"],
                    new TimeSlot(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2)),
                    100m, null, [], [], []));

                Assert.Equal(paramName, ex.ParamName);
            }

            // Time verification tests
            [Fact]
            public void Create_StartAfterEnd_ThrowsDomainException()
            {
                var start = DateTime.UtcNow.AddHours(2);
                var end = DateTime.UtcNow.AddHours(1);
                var timeSlot = new TimeSlot(start, end);

                Assert.Throws<DomainException>(() => Session.Create(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    timeSlot, 100m, null, [], [], []));
            }

            [Fact]
            public void Create_StartInPast_ThrowsDomainException()
            {
                Assert.Throws<DomainException>(() => Session.Create(
                    Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    new TimeSlot(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddHours(1)),
                    100m, null, [], [], []));
            }

            //Overlap validation tests
            [Fact]
            public void Create_ClientOverlap_ThrowsDomainException()
            {
                var start = DateTime.UtcNow.AddHours(2);
                var end = start.AddHours(1);
                var timeSlot = new TimeSlot(start, end);
                var existing = BuildSession(start, end);

                Assert.Throws<DomainException>(() => Session.Create(
                    existing.SessionClientID, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    timeSlot, 100m, null, [existing], [], []));
            }

            [Fact]
            public void Create_StaffOverlap_ThrowsDomainException()
            {
                var start = DateTime.UtcNow.AddHours(2);
                var end = start.AddHours(1);
                var timeSlot = new TimeSlot(start, end);
                var existing = BuildSession(start, end);

                Assert.Throws<DomainException>(() => Session.Create(
                    Guid.NewGuid(), existing.SessionStaffID, Guid.NewGuid(), Guid.NewGuid(),
                    timeSlot, 100m, null, [], [existing], []));
            }

            //Update Session Validation tests
            [Fact]
            public void UpdateSessionTime_ValidTimes_UpdatesSession()
            {
                var session = BuildSession();
                var newStart = DateTime.UtcNow.AddHours(3);
                var newEnd = newStart.AddHours(1);
                var timeSlot = new TimeSlot(newStart, newEnd);

            session.UpdateSessionTime(session.Id, timeSlot, [], [], []);

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

                Assert.Throws<InvalidOperationException>(() => session.CompletedSession());
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

                Assert.Throws<InvalidOperationException>(() => session.CancelSession());
            }

            [Fact]
            public void NoShowSession_ActiveSession_SetsNoShow()
            {
                var session = BuildSession();

                session.SetNoShowSession();

                Assert.Equal(SessionStatusEnum.NoShow, session.SessionStatus);
            }

    }
}
