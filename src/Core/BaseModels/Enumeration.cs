using System;

namespace Core.BaseModels
{
    public abstract class Enumeration<TKey> : Entity<TKey>, IEnumeration
        where TKey : Enum
    {
        private string _name = string.Empty;

        protected Enumeration() { }

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
