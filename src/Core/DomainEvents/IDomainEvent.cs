using MediatR;

namespace Core.DomainEvents
{
    /// <summary>
    /// Represents a domain event that implements the MediatR INotification interface.
    /// </summary>
    public interface IDomainEvent : INotification
    {
    }
}
