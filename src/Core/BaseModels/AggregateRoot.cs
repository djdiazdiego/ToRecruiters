using System.Collections.Generic;
using Core.DomainEvents;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents an aggregate root in the domain-driven design pattern.
    /// An aggregate root is an entity that serves as the entry point for accessing an aggregate.
    /// </summary>
    /// <typeparam name="TKey">The type of the unique identifier for the aggregate root.</typeparam>
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class.
        /// </summary>
        protected AggregateRoot() => _domainEvents = new List<IDomainEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the aggregate root.</param>
        protected AggregateRoot(TKey id) : base(id) => _domainEvents = new List<IDomainEvent>();

        /// <inheritdoc />
        public void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

        /// <inheritdoc />
        public void RemoveDomainEvent(IDomainEvent @event) => _domainEvents.Remove(@event);

        /// <inheritdoc />
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <inheritdoc />
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents;
    }
}
