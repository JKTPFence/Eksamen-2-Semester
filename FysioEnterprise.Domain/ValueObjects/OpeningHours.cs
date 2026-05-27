using System;
using System.Collections.Generic;
using System.Text;
using FysioEnterprise.Domain.Exceptions;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class OpeningHours : ValueObject
    {
        public DayOfWeek Day { get; }
        public TimeOnly From { get;}
        public TimeOnly To { get;}
        public bool IsClosed => Day.Equals(DayOfWeek.Sunday) || Day.Equals(DayOfWeek.Saturday) ||(From == TimeOnly.MinValue && To == TimeOnly.MinValue);

        private OpeningHours() { } // EF Core

        public OpeningHours(DayOfWeek day, TimeOnly from, TimeOnly to)
        {
            if (to <= from && (Day is not DayOfWeek.Sunday && Day is not DayOfWeek.Saturday))
                throw new DomainException($"Åbningstider må ikke slutte før de starter");
            Day = day;
            From = from;
            To = to;
        }
        public static OpeningHours Closed(DayOfWeek day)
        => new() { };

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return From;
            yield return To;
            yield return Day;
        }
    }
}
