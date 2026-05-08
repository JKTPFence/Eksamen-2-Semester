using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;

namespace FysioEnterprise.Domain.Entities
{
    public class Session
    {
        public Guid SessionID { get; private set; }
        public Guid SessionClientID { get; private set; }
        public Guid SessionStaffID { get; private set; }
        public Guid SessionRoomID { get; private set; }
        public Guid SessionInstanceTypeID { get; private set; }
        public Promotion? SessionPromotion { get; private set; }
        public DateTime SessionStartTime { get; private set; }
        public DateTime? SessionEndTime { get; private set; }
        public decimal? SessionTotalPrice { get; private set; }
        public SessionStatusEnum SessionStatus { get; private set; }

        public bool IsActive => SessionStatus == SessionStatusEnum.Active;

        private Session() { } // EF Core

        public static Session Create(
            Client client,
            Staff staff,
            SessionType sessionType,
            Room room,
            DateTime startTime,
            DateTime endTime,
            decimal totalPrice,
            Promotion? promotion,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (staff == null) throw new ArgumentNullException(nameof(staff));
            if (sessionType == null) throw new ArgumentNullException(nameof(sessionType));
            if (room == null) throw new ArgumentNullException(nameof(room));

            ValidateSessionTime(startTime, endTime);
            ValidateOverlap(existingClientSessions, startTime, endTime, "Client");
            ValidateOverlap(existingStaffSessions, startTime, endTime, "Staff");

            return new Session
            {
                SessionID = Guid.NewGuid(),
                SessionClientID = client.ClientID,
                SessionStaffID = staff.StaffID,
                SessionRoomID = room.RoomID,
                SessionStartTime = startTime,
                SessionEndTime = endTime,
                SessionTotalPrice = totalPrice,
                SessionStatus = SessionStatusEnum.Active,
                SessionInstanceTypeID = sessionType.SessionTypeId,
                SessionPromotion = promotion
            };
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
