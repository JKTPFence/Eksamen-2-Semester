using FysioEnterprise.Domain.Exceptions;

namespace FysioEnterprise.Domain.ValueObjects
{
    public record Price
    {
        public double Value { get; init; }
        private Price() // Empty constructor for EF Core
        {

        }
        public Price(double value)
        {
            if (value < 0) throw new UserInvalidInputException("Mængden må ikke være negativ");
            Value = value;
        }
    }
}
