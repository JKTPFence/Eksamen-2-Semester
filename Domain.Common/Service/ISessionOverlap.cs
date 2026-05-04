using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service
{
    public interface ISessionOverlap
    {
        bool HasOverlapClient(Guid clientId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        bool HasOverlapStaff(Guid staffId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    }
}
