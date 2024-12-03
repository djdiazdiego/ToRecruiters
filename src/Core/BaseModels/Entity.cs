using System;

namespace Core.BaseModels
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey> : IEntity
    {
        private int? _requestedHashCode;

        protected Entity()
        {
        }

        protected Entity(TKey id) => Id = id;

        /// <summary>
        /// Entity identifier
        /// </summary>
        public TKey Id { get; private set; }

        /// <inheritdoc />
        object IEntity.Id => Id;
        /// <inheritdoc />
        public DateTimeOffset CreationDate { get; private set; }
        /// <inheritdoc />
        public DateTimeOffset? LastUpdateDate { get; private set; }

        /// <inheritdoc />
        public bool IsTransient() => (typeof(TKey) == typeof(long) || typeof(TKey) == typeof(int) || typeof(TKey) == typeof(Guid)) &&
            Id.Equals(default(TKey));

        /// <inheritdoc />
        public override bool Equals(object obj) => obj != null && obj is Entity<TKey> entity &&
            (ReferenceEquals(this, obj) || GetType() == obj.GetType() && !entity.IsTransient() && !IsTransient() && entity.Id.Equals(Id));

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            if (!_requestedHashCode.HasValue)
                _requestedHashCode = new int?(Id.GetHashCode() ^ 31);

            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) =>
            Equals(left, null) ? Equals(right, null) : left.Equals(right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);

        protected void SetCreationDate(DateTimeOffset? creationDate = null) =>
            CreationDate = creationDate ?? DateTimeOffset.UtcNow;

        protected void SetLastUpdateDate(DateTimeOffset? lastUpdateDate = null) =>
            LastUpdateDate = lastUpdateDate ?? DateTimeOffset.UtcNow;
    }
}
