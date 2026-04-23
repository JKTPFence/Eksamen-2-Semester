using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.Service
{
    public interface IBookingOverlap
    {
        bool HasOverlap(Guid clientId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    }
}
