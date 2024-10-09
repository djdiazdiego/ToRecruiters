using MediatR;

namespace Core.Features
{
    public abstract class Command<TResponse> : IRequest<TResponse>
        where TResponse : class
    {
    }

    public abstract class Command : IRequest
    {
    }
}
