using FysioEnterprise.Domain.Enums;
using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Commands.SessionComands
{
    public interface IUpdateSessionCommand
    {
        Task UpdateSessionAsync(UpdateSessionCommand command);

        public record UpdateSessionCommand(Guid SessionId, Guid ClientId, Guid StaffId, DateTime StartTime, DateTime EndTime, Guid SessionRoom, SessionStatusEnum SessionStatus, SessionType SessionType, Guid PromotionID, int? SessionTotalPrice);
    }
}
