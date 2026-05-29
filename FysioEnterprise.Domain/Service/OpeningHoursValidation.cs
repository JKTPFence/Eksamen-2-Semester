using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service
{
    public class OpeningHoursValidation
    {
        public static bool IsOutsideOpeningHours(DateTime startTime, DateTime endTime, List<OpeningHours>? openingHours) //Used to check if the given session time is outside the opening hours of the clinic. Returns true if it is outside, false otherwise.
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
