using System.Collections;
using FluentResults;
using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.Exceptions;
using FysioEnterprise.Domain.Service;
using FysioEnterprise.Domain.Service.PricingService;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Entities
{
    public class Session : Aggregateroot
    {
        public Guid SessionClientID { get; init; }
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
            if (clientId == Guid.Empty) throw new UserInvalidInputException("For at oprette en booking skal der være en klient");
            SessionClientID = clientId;
            if (staffId == Guid.Empty) throw new UserInvalidInputException("For at oprette en booking skal der være en medarbejder");
            SessionStaffID = staffId;
            if (sessionTypeId == Guid.Empty) throw new UserInvalidInputException("For at oprette en booking skal der være en bookingtype");
            SessionInstanceTypeID = sessionTypeId;
            if (roomId == Guid.Empty) throw new UserInvalidInputException("For at oprette en booking skal der vælges et rum");
            SessionRoomID = roomId;
            SessionPromotion = promotionId;
            SessionStatus = SessionStatusEnum.Active;

        }

        public static Session Create(
            Client client,
            Staff staff,
            SessionType sessionType,
            Guid roomId,
            TimeSlot sessionTimeSlot,
            Promotion? promotion,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions,
            IPricingStrategyFactory pricingStrategyFactory,
            List<OpeningHours> openingHours)
        {

            var newSession = new Session(client.Id, staff.Id, sessionType.Id, roomId, promotion?.Id, sessionTimeSlot);

            ValidateOverlap(newSession.SessionTimeSlot, existingClientSessions, existingStaffSessions, existingRoomSessions);
            var result = TimeValidationService.ValidateTime(sessionType.SessionTypeName, newSession.SessionTimeSlot.From, newSession.SessionTimeSlot.To, DateTime.Now);

            if (result.IsFailed)
                throw new ValidationException("Fejl med validering af tid " + result.Errors.First().Message);

            if (sessionTimeSlot.From < promotion?.PromotionStartTime || sessionTimeSlot.To > promotion?.PromotionEndTime)
            {
                promotion = null; // Promotion is not valid for the selected session (We dont want to validate the promotion for now but for when the session is active), so we set it to null to ensure it is not applied to the session.
            }

            newSession.priceTotal = pricingStrategyFactory.BuildStrategies(client,
                promotion,
                sessionType).Result;
            var basePrice = newSession.priceTotal;

            //After calculating the base price with the pricing strategy, we apply the staff multiplier and the outside of opening hours multiplier if applicable.
            //This is done after the pricing strategy to ensure that the multipliers are applied to the final price after all discounts and promotions have been applied.
            var authType = AuthorisationTypeExtensions.FromRoleString(staff.StaffAuthorisationType); 
            double staffMultiplier = authType.GetPriceMultiplier();
            var firstCalculatedPrice = new Price(basePrice.Value * staffMultiplier);

            var IsOutsideOf = OpeningHoursValidation.IsOutsideOpeningHours(sessionTimeSlot.From, sessionTimeSlot.To, openingHours);
            if (IsOutsideOf)
            {
                firstCalculatedPrice = new Price(firstCalculatedPrice.Value * 1.15);
            }

            newSession.priceTotal = new Price(firstCalculatedPrice.Value);

            return newSession;
        }

        public void UpdateSession(
            Client client,
            Staff staff,
            SessionType sessionType,
            Guid roomId,
            TimeSlot sessionTimeSlot,
            Promotion? promotion,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions,
            IPricingStrategyFactory pricingStrategyFactory,
            List<OpeningHours> openingHours)
        {
            if (!IsActive)
                throw new UserInvalidInputException($"Der kan ikke laves ændringer i en ikke aktiv tid");

            ValidateOverlap(sessionTimeSlot, existingClientSessions, existingStaffSessions, existingRoomSessions, this.Id);
            var result = TimeValidationService.ValidateTime("New time", sessionTimeSlot.From, sessionTimeSlot.To, DateTime.Now);

            if (result.IsFailed)
                throw new ValidationException("Fejl med validering af tid " + result.Errors.First().Message);

            if (sessionTimeSlot.From < promotion?.PromotionStartTime || sessionTimeSlot.To > promotion?.PromotionEndTime)
                promotion = null;

            SessionStaffID = staff.Id;
            SessionInstanceTypeID = sessionType.Id;
            SessionRoomID = roomId;
            SessionPromotion = promotion?.Id;
            SessionTimeSlot = sessionTimeSlot;

            var basePrice = pricingStrategyFactory.BuildStrategies(client, promotion, sessionType).Result;
            var authType = AuthorisationTypeExtensions.FromRoleString(staff.StaffAuthorisationType);
            double staffMultiplier = authType.GetPriceMultiplier();
            var calculatedPrice = new Price(basePrice.Value * staffMultiplier);

            if (OpeningHoursValidation.IsOutsideOpeningHours(sessionTimeSlot.From, sessionTimeSlot.To, openingHours))
                calculatedPrice = new Price(calculatedPrice.Value * 1.15);

            priceTotal = calculatedPrice;
        }

        public void CompletedSession()
        {
            if (!IsActive)
                throw new UserInvalidInputException("Kan ikke afslutte en ikke aktiv tid");

            SessionStatus = SessionStatusEnum.Completed;
        }

        public void CancelSession()
        {
            if (!IsActive)
                throw new UserInvalidInputException("Kan ikke afmelde en ikke aktiv tid");
            SessionStatus = SessionStatusEnum.Cancelled;
        }

        public void SetNoShowSession()
        {
            SessionStatus = SessionStatusEnum.NoShow;
        }

        private static void ValidateOverlap(
            TimeSlot sessionTimeSlot,
            IEnumerable<Session> existingClientSessions,
            IEnumerable<Session> existingStaffSessions,
            IEnumerable<Session> existingRoomSessions,
            Guid? currentSessionId = null
            ) //Validation method to check if the new or updated session overlaps with any existing sessions for the client, staff, or room. It checks across all clinics
        {
            var clientOverlap = existingClientSessions
            .Where(c => c.Id != currentSessionId)
            .Where(c => c.IsActive)
            .FirstOrDefault(c => 
                sessionTimeSlot.OverlapWithOtherTimeSlot(c.SessionTimeSlot)
                && c.SessionTimeSlot.To != sessionTimeSlot.From
                && c.SessionTimeSlot.From != sessionTimeSlot.To
            );

            if (clientOverlap is not null)
            {
                    throw new ValidationException(
                        $"Klient har allerede en booking"
                        + $"({clientOverlap.SessionTimeSlot.From:HH:mm}-{clientOverlap.SessionTimeSlot.To:HH:mm}) "
                        + $"der overlapper med nuværende booking");
            }

            var staffOverlap = existingStaffSessions
                .Where(p => p.Id != currentSessionId)
                .Where(p => p.IsActive)
                .FirstOrDefault(p => 
                    sessionTimeSlot.OverlapWithOtherTimeSlot(p.SessionTimeSlot)
                    && p.SessionTimeSlot.To != sessionTimeSlot.From
                    && p.SessionTimeSlot.From != sessionTimeSlot.To
                );

            if (staffOverlap is not null)
            {
                    throw new ValidationException(
                    $"Denne medarbejder har allerede en booking"
                    + $"({staffOverlap.SessionTimeSlot.From:HH:mm}-{staffOverlap.SessionTimeSlot.To:HH:mm}) "
                    + $"som overlapper med en eksisterende booking");
            }

            var roomOverlap = existingRoomSessions
            .Where(c => c.Id != currentSessionId)
            .Where(c => c.IsActive)
            .FirstOrDefault(c => 
                sessionTimeSlot.OverlapWithOtherTimeSlot(c.SessionTimeSlot)
                && c.SessionTimeSlot.To != sessionTimeSlot.From
                && c.SessionTimeSlot.From != sessionTimeSlot.To
            );

            if (roomOverlap is not null)
            {
                    throw new ValidationException(
                    $"Dette rum er optaget af en anden booking "
                    + $"({roomOverlap.SessionTimeSlot.From:HH:mm}-{roomOverlap.SessionTimeSlot.To:HH:mm}) "
                    + $"hvilket lavet et booking overlap");
            }


        }

    }

}
