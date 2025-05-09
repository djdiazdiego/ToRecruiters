using System;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents a base class for enumerations with a unique identifier and a name.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier for the enumeration.</typeparam>
    public abstract class Enumeration<TKey> : Entity<TKey>, IEnumeration
        where TKey : Enum
    {
        private string _name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumeration{TKey}"/> class.
        /// </summary>
        protected Enumeration() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumeration{TKey}"/> class with the specified identifier and name.
        /// </summary>
        /// <param name="id">The unique identifier for the enumeration.</param>
        /// <param name="name">The name of the enumeration.</param>
        protected Enumeration(TKey id, string name) : base(id)
        {
            _name = name;
        }

        /// <inheritdoc />
        public string Name => _name;

        /// <inheritdoc />
        public void SetName(string name) => _name = name;

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is Enumeration<TKey> otherValue && GetType().Equals(obj.GetType()) && Id.Equals(otherValue.Id);

        /// <inheritdoc />
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Name;

        /// <inheritdoc />
        public int CompareTo(object other) => Id.CompareTo(((Enumeration<TKey>)other).Id);
    }
}
