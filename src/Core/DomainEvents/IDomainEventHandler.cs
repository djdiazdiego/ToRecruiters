using MediatR;

namespace Core.DomainEvents
{
    public interface IDomainEventHandler<TEvent> : INotificationHandler<IDomainEvent>
        where TEvent : IDomainEvent
    {
    }
}
