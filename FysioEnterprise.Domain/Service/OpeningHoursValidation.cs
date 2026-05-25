using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service
{
    public class OpeningHoursValidation
    {
        public static bool IsOutsideOpeningHours(DateTime startTime, DateTime endTime, List<OpeningHours>? openingHours)
        {
            if (openingHours is null) return false;

            var dayOfWeek = startTime.DayOfWeek;
            var oh = openingHours.FirstOrDefault(o => o.Day == dayOfWeek);

            if (oh is null || oh.IsClosed) return true;

            var sessionStart = TimeOnly.FromDateTime(startTime);
            var sessionEnd = TimeOnly.FromDateTime(endTime);

            return sessionStart < oh.From || sessionEnd > oh.To;
        }
    }
}
