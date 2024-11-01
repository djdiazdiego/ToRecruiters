using System.Collections.Generic;
using Core.DomainEvents;

namespace Core.BaseModels
{
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Clear domain event
        /// </summary>
        void ClearDomainEvents();

        /// <summary>
        /// Add domain event
        /// </summary>
        /// <param name="event"></param>
        void AddDomainEvent(IDomainEvent @event);

        /// <summary>
        /// Remove domain event
        /// </summary>
        /// <param name="event"></param>
        void RemoveDomainEvent(IDomainEvent @event);

        /// <summary>
        /// Get domain events from an aggregate
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    }
}
