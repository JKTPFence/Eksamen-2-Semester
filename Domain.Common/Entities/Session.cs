using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;

namespace FysioEnterprise.Domain.Entities
{
    public class Session : Aggregateroot
    {

        public Guid SessionClientID { get; private set; }
        public Guid SessionStaffID { get; private set; }
        public Guid SessionRoomID { get; private set; }
        public Guid SessionInstanceTypeID { get; private set; }
        public Guid? SessionPromotion { get; private set; }
        public DateTime SessionStartTime { get; private set; }
        public DateTime? SessionEndTime { get; private set; }
        public decimal? SessionTotalPrice { get; private set; }
        public SessionStatusEnum SessionStatus { get; private set; }

        public bool IsActive => SessionStatus == SessionStatusEnum.Active;

        private Session() { } // EF Core

        private Session(
            Guid clientId,
            Guid staffId,
            Guid sessionTypeId,
            Guid roomId,
            Guid? promotionId,
            DateTime startTime,
            DateTime endTime
        )
        {
            Id = Guid.NewGuid();
            if (clientId == Guid.Empty) throw new DomainException(nameof(clientId));
            if (staffId == Guid.Empty) throw new DomainException(nameof(staffId));
            if (sessionTypeId == Guid.Empty) throw new DomainException(nameof(sessionTypeId));
            if (roomId == Guid.Empty) throw new DomainException(nameof(roomId));
        }

        public static Session Create(
            Guid clientId,
            Guid staffId,
            Guid sessionTypeId,
            Guid roomId,
            DateTime startTime,
            DateTime endTime,
            decimal totalPrice,
            Guid? promotionId,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions)
        {

            var Session = new Session (clientId, staffId, sessionTypeId, roomId, promotionId, startTime, endTime);

            ValidateSessionTime(startTime, endTime);
            ValidateOverlap(existingClientSessions, startTime, endTime, "Client");
            ValidateOverlap(existingStaffSessions, startTime, endTime, "Staff");

            return Session;
        }

        public void UpdateSessionTime(
            DateTime newStartTime,
            DateTime newEndTime,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions)
        {
            if (!IsActive)
                throw new InvalidOperationException($"Cannot update time of an inactive session.");

            ValidateSessionTime(newStartTime, newEndTime);
            ValidateOverlap(existingClientSessions, newStartTime, newEndTime, "Client");
            ValidateOverlap(existingStaffSessions, newStartTime, newEndTime, "Staff");

            SessionStartTime = newStartTime;
            SessionEndTime = newEndTime;
        }

        public void CompletedSession()
        {
            if (!IsActive)
                throw new InvalidOperationException($"Cannot complete a non active session.");

            SessionStatus = SessionStatusEnum.Completed;
        }

        public void CancelSession()
        {
            if (!IsActive)
                throw new InvalidOperationException($"Cannot cancel a non active session.");
            SessionStatus = SessionStatusEnum.Cancelled;
        }

        private static void ValidateSessionTime(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new DomainException("Session start time must be before end time.");
            if (startTime < DateTime.UtcNow)
                throw new DomainException("Session start time cannot be in the past.");
        }

        private static void ValidateOverlap(
            IEnumerable<Session> existingSessions,
            DateTime startTime,
            DateTime endTime,
            string ownerType)
        {
            var overlap = existingSessions
                .Where(s => s.IsActive)
                .FirstOrDefault(s =>
                    startTime < s.SessionEndTime &&
                    endTime > s.SessionStartTime);

            if (overlap != null)
                throw new DomainException(
                    $"{ownerType} already has a session " +
                    $"({overlap.SessionStartTime:HH:mm}-{overlap.SessionEndTime:HH:mm}) " +
                    $"that overlaps with the requested time.");
        }
    }

}
