using System.Collections.Generic;
using Core.DomainEvents;

namespace Core.BaseModels
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents;

        protected AggregateRoot() => _domainEvents = new List<IDomainEvent>();
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
