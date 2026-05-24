using FysioEnterprise.Domain.Exceptions;

namespace FysioEnterprise.Domain.ValueObjects
{
    public class TimeSlot : ValueObject
    {
        public DateTime From { get; init; }
        public DateTime To { get; init; }

        private TimeSlot() { } // Empty Constructor for EF Core

        public TimeSlot(DateTime from, DateTime to)
        {
            if (to <= from)
                throw new DomainException("Til tiden må ikke være før fra tiden");

        From = from;
        To = to;
        }

        public TimeSpan TimeSpan => To - From;

        public bool OverlapWithOtherTimeSlot(TimeSlot other)
            => From < other.To && other.From < To;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return From;
            yield return To;
        }
    }
}
