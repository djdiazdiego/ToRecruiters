using MediatR;

namespace Core.Features
{
    public abstract class Query<TResponse> : IRequest<TResponse>
        where TResponse : class
    {
    }

    public abstract class Query : IRequest
    {
    }
}
