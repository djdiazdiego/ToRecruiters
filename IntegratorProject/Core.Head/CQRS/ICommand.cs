using MediatR;

namespace Core.Head.CQRS
{
    public interface ICommand<TResponse> : IRequest<TResponse>
        where TResponse : class
    {
    }

    public interface ICommand : IRequest
    {
    }
}
