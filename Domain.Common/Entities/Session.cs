using FysioEnterprise.Domain.ValueObjects;
using FysioEnterprise.Domain.Service;
using System.ComponentModel.DataAnnotations;
using FysioEnterprise.Domain.Enums;

namespace FysioEnterprise.Domain.Entities
{
    public class Session
    {
        public Guid SessionID { get; private set; }
        public Guid SessionClientID { get; private set; }
        public Guid SessionStaffID { get; private set; }
        public Guid SessionRoomID { get; private set; }
        public SessionType SessionInstanceType{ get; private set; }
        public Promotion? SessionPromotion { get; private set; }
        public DateTime SessionStartTime { get; private set; }
        public DateTime? SessionEndTime { get; private set; }
        public int? SessionTotalPrice { get; private set; }
        public SessionStatusEnum SessionStatus { get; private set; }
        
        public Session() // Empty constructor for EF Core
        {
            
        }

        public Session(Client client, Staff staff, SessionType sessionType, Room room, DateTime startTime, DateTime endTime, int? totalPrice, SessionStatusEnum status, Promotion? promotion, ISessionOverlap bookingOverlap, ITimeNow timeNow)
        {
            if(client == null) throw new ArgumentNullException(nameof(client));
            if(staff == null) throw new ArgumentNullException(nameof(staff));
            if(sessionType == null) throw new ArgumentNullException(nameof(sessionType));
            if(room == null) throw new ArgumentNullException(nameof(room));
            ValidateSessionTime(startTime, endTime, timeNow);
            SessionID = Guid.NewGuid();
            SessionClientID = client.ClientID;
            SessionStaffID = staff.StaffID;
            SessionRoomID = room.RoomID;
            SessionStartTime = startTime;
            SessionEndTime = endTime;
            SessionTotalPrice = totalPrice;
            SessionStatus = status;
            SessionInstanceType = sessionType;
            SessionPromotion = promotion;
        }
        private static void ValidateSessionTime(DateTime sessionStartTime, DateTime sessionEndTime, ITimeNow timeNow)
        {
            if (sessionStartTime >= sessionEndTime) 
                throw new ArgumentException("Session start time must be before end time.");
            if (sessionStartTime < timeNow.Now()) 
                throw new ArgumentException("Session start time cannot be in the past.");
        }

        public void CancelSession()
        {
            if (SessionStatus is not SessionStatusEnum.Active)
                throw new InvalidOperationException($"Cannot cancel a non active session.");
            SessionStatus = SessionStatusEnum.Cancelled;
        }

            
    }
}
