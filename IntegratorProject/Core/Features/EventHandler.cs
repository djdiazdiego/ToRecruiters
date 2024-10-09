using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features
{
    public abstract class EventHandler<TEvent> : INotificationHandler<Event>
        where TEvent : Event
    {
        public abstract Task Handle(Event notification, CancellationToken cancellationToken);
    }
}
