using FluentResults;

namespace FysioEnterprise.Domain.Service
{
    public static class TimeValidationService
    {
        public static Result ValidateTime(
            string eventName,
            DateTime startTime,
            DateTime endTime,
            DateTime currentTime)
        {
            if (startTime >= endTime)
                return Result.Fail($"{eventName} must start before it ends.");
            
            if (startTime < currentTime)
                return Result.Fail($"{eventName} start cannot be in the past.");

            return Result.Ok();
        }
    }
}
