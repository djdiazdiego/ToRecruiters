using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents a base class for value objects, providing equality comparison logic.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Determines whether two <see cref="ValueObject"/> instances are equal using the equality operator.
        /// </summary>
        /// <param name="left">The left <see cref="ValueObject"/> instance.</param>
        /// <param name="right">The right <see cref="ValueObject"/> instance.</param>
        /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right) =>
            !(left is null ^ right is null) && (left is null || left.Equals(right));

        /// <summary>
        /// Determines whether two <see cref="ValueObject"/> instances are not equal using the inequality operator.
        /// </summary>
        /// <param name="left">The left <see cref="ValueObject"/> instance.</param>
        /// <param name="right">The right <see cref="ValueObject"/> instance.</param>
        /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
            !EqualOperator(left, right);

        /// <summary>
        /// Provides the components that are used to determine equality for the value object.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of components used for equality comparison.</returns>
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
