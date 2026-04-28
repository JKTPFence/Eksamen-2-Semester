using FysioEnterprise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Port.Driving.Queries
{
    public interface ISessionQuery
    {
        /// <summary>
        ///     Som receptionist vil jeg kunne se alle sessioner for en given kunde, så jeg kan få et overblik over kundens bookinger.
        /// </summary>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        List<SessionDto> GetAllBySessionId(Guid ClientId);
    }

    public record SessionDto(Guid SessionId, Guid ClientId, Guid StaffId, DateTime StartTime, DateTime EndTime, Guid SessionRoom, Enum SessionStatus, SessionType SessionType, Guid PromotionID, int? SessionTotalPrice);
    public record GetAllByClientRequest(Guid ClientId);
}
