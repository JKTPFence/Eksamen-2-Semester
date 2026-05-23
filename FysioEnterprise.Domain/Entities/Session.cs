using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Session : Aggregateroot
    {

        public Guid SessionClientID { get; private set; }
        public Guid SessionStaffID { get; private set; }
        public Guid SessionRoomID { get; private set; }
        public Guid SessionInstanceTypeID { get; private set; }
        public Guid? SessionPromotion { get; private set; }
        public TimeSlot SessionTimeSlot { get; private set; } = null!;
        public SessionStatusEnum SessionStatus { get; private set; }
        public Price priceTotal { get; private set; } = new(0);
        public bool IsActive => SessionStatus == SessionStatusEnum.Active;

        private Session() { } // EF Core

        private Session(
            Guid clientId,
            Guid staffId,
            Guid sessionTypeId,
            Guid roomId,
            Guid? promotionId,
            TimeSlot sessionTimeSlot
        )
        {
            Id = Guid.NewGuid();
            SessionTimeSlot = sessionTimeSlot;
            if (clientId == Guid.Empty) throw new DomainException(nameof(clientId));
            SessionClientID = clientId;
            if (staffId == Guid.Empty) throw new DomainException(nameof(staffId));
            SessionStaffID = staffId;
            if (sessionTypeId == Guid.Empty) throw new DomainException(nameof(sessionTypeId));
            SessionInstanceTypeID = sessionTypeId;
            if (roomId == Guid.Empty) throw new DomainException(nameof(roomId));
            SessionRoomID = roomId;
            SessionPromotion = promotionId;
            SessionStatus = SessionStatusEnum.Active;

        }

        public static Session Create(
            Client client,
            Guid staffId,
            SessionType sessionType,
            Guid roomId,
            TimeSlot sessionTimeSlot,
            Promotion? promotion,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions,
            IPricingStrategyFactory pricingStrategyFactory)
        {

            var newSession = new Session(client.Id, staffId, sessionType.Id, roomId, promotion?.Id, sessionTimeSlot);

            ValidateOverlap(newSession.SessionTimeSlot, existingClientSessions, existingStaffSessions, existingRoomSessions);
            TimeValidationService.ValidateTime(sessionType.SessionTypeName, newSession.SessionTimeSlot.From, newSession.SessionTimeSlot.To, DateTime.Now);

            if (sessionTimeSlot.From < promotion?.PromotionStartTime || sessionTimeSlot.To > promotion?.PromotionEndTime)
            {
                promotion = null; // Promotion is not valid for the selected session (We dont want to validate the promotion for now but for when the session is active), so we set it to null to ensure it is not applied to the session.
            }

            newSession.priceTotal = pricingStrategyFactory.BuildStrategies(client,
                promotion,
                sessionType).Result;

            return newSession;
        }

        public void UpdateSessionTime(
            Guid sessionId,
            TimeSlot newSessionTimeSlot,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions)
        {
            if (!IsActive)
                throw new UserInvalidInputException($"Cannot update time of an inactive session.");

            ValidateOverlap(newSessionTimeSlot, existingClientSessions, existingStaffSessions, existingRoomSessions, sessionId);
            TimeValidationService.ValidateTime("New time", newSessionTimeSlot.From, newSessionTimeSlot.To, DateTime.Now);

            SessionTimeSlot = newSessionTimeSlot;
        }

        public void CompletedSession()
        {
            if (!IsActive)
                throw new UserInvalidInputException($"Cannot complete a non active session.");

            SessionStatus = SessionStatusEnum.Completed;
        }

        public void CancelSession()
        {
            if (!IsActive)
                throw new UserInvalidInputException($"Cannot cancel a non active session.");
            SessionStatus = SessionStatusEnum.Cancelled;
        }

        public void SetNoShowSession()
        {
            if (SessionStatus == SessionStatusEnum.Completed)
                throw new UserInvalidInputException($"Cannot set a completed session to no show.");
            SessionStatus = SessionStatusEnum.NoShow;
        }

        private static void ValidateOverlap(
            TimeSlot sessionTimeSlot,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions,
            Guid? currentSessionId = null
            )
        {
            var clientOverlap = existingClientSessions
            .Where(c => c.Id != currentSessionId)
            .Where(c => c.IsActive)
            .FirstOrDefault(c => sessionTimeSlot.OverlapWithOtherTimeSlot(c.SessionTimeSlot));

            if (clientOverlap is not null)
            {
                throw new DomainException(
                    $"Client already has a session "
                    + $"({clientOverlap.SessionTimeSlot.From:HH:mm}-{clientOverlap.SessionTimeSlot.To:HH:mm}) "
                    + $"that overlaps with current booking");
            }

            var staffOverlap = existingStaffSessions
                .Where(p => p.Id != currentSessionId)
                .Where(p => p.IsActive)
                .FirstOrDefault(p => sessionTimeSlot.OverlapWithOtherTimeSlot(p.SessionTimeSlot));

            if (staffOverlap is not null)
            {
                throw new DomainException(
                    $"Staff already has a session "
                    + $"({staffOverlap.SessionTimeSlot.From:HH:mm}-{staffOverlap.SessionTimeSlot.To:HH:mm}) "
                    + $"that overlaps with current booking");
            }

            var roomOverlap = existingRoomSessions
            .Where(c => c.Id != currentSessionId)
            .Where(c => c.IsActive)
            .FirstOrDefault(c => sessionTimeSlot.OverlapWithOtherTimeSlot(c.SessionTimeSlot));

            if (roomOverlap is not null)
            {
                throw new DomainException(
                    $"Room is currently occupied by a different session "
                    + $"({roomOverlap.SessionTimeSlot.From:HH:mm}-{roomOverlap.SessionTimeSlot.To:HH:mm}) "
                    + $"this overlaps with current booking");
            }
        }

    }

}
