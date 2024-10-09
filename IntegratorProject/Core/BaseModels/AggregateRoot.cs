using Core.BaseModels.Interfaces;
using Core.Features;
using System.Collections.Generic;

namespace Core.BaseModels
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        private readonly List<Event> _domainEvents;

        protected AggregateRoot() => _domainEvents = new List<Event>();
        protected AggregateRoot(TKey id) : base(id) => _domainEvents = new List<Event>();

        /// <inheritdoc />
        public void AddDomainEvent(Event @event) => _domainEvents.Add(@event);

        /// <inheritdoc />
        public void RemoveDomainEvent(Event @event) => _domainEvents.Remove(@event);

        /// <inheritdoc />
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <inheritdoc />
        public IReadOnlyCollection<Event> GetDomainEvents() => _domainEvents;
    }
}
