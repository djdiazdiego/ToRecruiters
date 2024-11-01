using MediatR;

namespace Core.Head.CQRS
{
    public interface IQuery<TResponse> : IRequest<TResponse>
        where TResponse : class
    {
    }
}
