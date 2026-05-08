using FysioEnterprise.Domain.Entities;

namespace FysioEnterprise.Facade.RequestModels
{
    public class SessionRequests
    {
        public record CreateSessionRequest(
        Guid ClientID,
        Guid StaffID,
        Guid PromotionID,
        Guid SessionRoomID,
        Guid SessionInstanceTypeID,
        int SessionTotalPrice,
        DateTime StartTime,
        DateTime EndTime);

        public record UpdateSessionRequest(
            Guid SessionID,
            Guid ClientID,
            Guid StaffID,
            DateTime StartTime,
            DateTime EndTime);
        public record DeleteSessionRequest(
            Guid SessionID);
        public record EndSessionRequest(
            Guid SessionId, string Note);
        public record CancelSessionRequest(
            Guid SessionId);

        public record SearchSessionRequest(
     Guid SessionID,
     Guid ClientID,
     Guid StaffID,
     DateTime StartTime,
     DateTime EndTime);

      public record GetSessionRequest(Guid SessionID);

    }
}
