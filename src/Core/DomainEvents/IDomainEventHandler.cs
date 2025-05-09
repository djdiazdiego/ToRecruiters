using MediatR;

namespace Core.DomainEvents
{
    /// <summary>
    /// Represents a handler for a specific type of domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event being handled.</typeparam>
    public interface IDomainEventHandler<TEvent> : INotificationHandler<IDomainEvent>
        where TEvent : IDomainEvent
    {
    }
}
