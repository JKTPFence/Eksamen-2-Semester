using System;
using System.Collections.Generic;
using System.Text;

namespace FysioEnterprise.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        public override bool Equals(object? obj)
            => obj is Entity other && Id == other.Id;

        public override int GetHashCode()
            => Id.GetHashCode();
    }
    public abstract class Aggregateroot : Entity
    {

    }

    //Not an entity but it is a part of the hirarchy, and is kept here for the sake of organization and clarity
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return GetEqualityComponents()
                .SequenceEqual(((ValueObject)obj).GetEqualityComponents());
        }

        public override int GetHashCode()
            => GetEqualityComponents()
                .Aggregate(0, HashCode.Combine);
    }
}
