using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain.ValueObjects
{
    public record Price
    {
        public decimal Value { get; init; }
        private Price() // Empty constructor for EF Core
        {
        }
        public Price(decimal value)
        {
            if (value < 0) throw new ArgumentException("Amount cannot be negative.", nameof(value));
            Value = value;
        }
    }
}
