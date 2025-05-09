using System.Collections.Generic;
using Core.DomainEvents;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents an aggregate root in the domain-driven design pattern.
    /// An aggregate root is an entity that serves as the entry point for accessing an aggregate.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Clears all domain events associated with the aggregate root.
        /// </summary>
        void ClearDomainEvents();

        /// <summary>
        /// Adds a domain event to the aggregate root.
        /// </summary>
        /// <param name="event">The domain event to add.</param>
        void AddDomainEvent(IDomainEvent @event);

        /// <summary>
        /// Removes a specific domain event from the aggregate root.
        /// </summary>
        /// <param name="event">The domain event to remove.</param>
        void RemoveDomainEvent(IDomainEvent @event);

        /// <summary>
        /// Retrieves all domain events associated with the aggregate root.
        /// </summary>
        /// <returns>A read-only collection of domain events.</returns>
        IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    }
}
