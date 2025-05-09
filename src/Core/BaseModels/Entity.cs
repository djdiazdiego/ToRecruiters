using System;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents a base entity with a unique identifier and tracking for creation and update dates.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
    public abstract class Entity<TKey> : IEntity
    {
        private int? _requestedHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class.
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TKey}"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the entity.</param>
        protected Entity(TKey id) => Id = id;

        /// <inheritdoc />
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

        /// <summary>
        /// Determines whether two entities are equal.
        /// </summary>
        /// <param name="left">The first entity to compare.</param>
        /// <param name="right">The second entity to compare.</param>
        /// <returns><c>true</c> if the entities are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) =>
            Equals(left, null) ? Equals(right, null) : left.Equals(right);

        /// <summary>
        /// Determines whether two entities are not equal.
        /// </summary>
        /// <param name="left">The first entity to compare.</param>
        /// <param name="right">The second entity to compare.</param>
        /// <returns><c>true</c> if the entities are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);

        /// <summary>
        /// Sets the creation date of the entity.
        /// </summary>
        /// <param name="creationDate">The creation date to set. If <c>null</c>, the current UTC date and time will be used.</param>
        protected void SetCreationDate(DateTimeOffset? creationDate = null) =>
            CreationDate = creationDate ?? DateTimeOffset.UtcNow;

        /// <summary>
        /// Sets the last update date of the entity.
        /// </summary>
        /// <param name="lastUpdateDate">The last update date to set. If <c>null</c>, the current UTC date and time will be used.</param>
        protected void SetLastUpdateDate(DateTimeOffset? lastUpdateDate = null) =>
            LastUpdateDate = lastUpdateDate ?? DateTimeOffset.UtcNow;
    }
}
