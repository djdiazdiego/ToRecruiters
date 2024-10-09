using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.BaseModels
{
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right) =>
            !(left is null ^ right is null) && (left is null || left.Equals(right));

        protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
            !EqualOperator(left, right);

        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj != null && obj.GetType() == GetType() &&
            GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());

        /// <inheritdoc />
        public override int GetHashCode() => GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}
