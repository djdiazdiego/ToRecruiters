using Core.Features;
using System.Collections.Generic;

namespace Core.BaseModels.Interfaces
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
        void AddDomainEvent(Event @event);

        /// <summary>
        /// Remove domain event
        /// </summary>
        /// <param name="event"></param>
        void RemoveDomainEvent(Event @event);

        /// <summary>
        /// Get domain events from an aggregate
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Event> GetDomainEvents();
    }
}
