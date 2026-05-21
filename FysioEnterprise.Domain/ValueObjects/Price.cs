using System;
using System.Collections.Generic;
using System.Text;

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
            if (value < 0) throw new ArgumentException("Amount cannot be negative.", nameof(value));
            Value = value;
        }
    }
}
