using FluentResults;
using FysioEnterprise.Domain.ValueObjects;

namespace FysioEnterprise.Domain.Service
{
    public static class TimeValidationService
    {
        public static Result ValidateTime(
            string eventName,
            DateTime startTime,
            DateTime endTime,
            DateTime currentTime,
            List<OpeningHours>? openingHours)
        {
            if (startTime >= endTime)
                return Result.Fail($"{eventName} must start before it ends.");
            
            if (startTime < currentTime && eventName is not "Promotion")
                return Result.Fail($"{eventName} start cannot be in the past.");

            if (openingHours is not null)
            {
                var dayOfWeek = startTime.DayOfWeek;
                var oh = openingHours.FirstOrDefault(o => o.Day == dayOfWeek);

                if (oh is null)
                    return Result.Fail($"{eventName} cannot be scheduled — no opening hours defined for {dayOfWeek}.");

                if (oh.IsClosed)
                    return Result.Fail($"{eventName} cannot be scheduled — the clinic is closed on {dayOfWeek}.");

                var sessionStart = TimeOnly.FromDateTime(startTime);
                var sessionEnd = TimeOnly.FromDateTime(endTime);

                if (sessionStart < oh.From)
                    return Result.Fail($"{eventName} cannot start before opening hours ({oh.From:HH:mm}).");

                if (sessionEnd > oh.To)
                    return Result.Fail($"{eventName} cannot end after closing hours ({oh.To:HH:mm}).");
            }

            return Result.Ok();
        }
    }
}
