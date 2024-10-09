using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features
{
    public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : Query<TResponse>
        where TResponse : class
    {
        public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
    }
}
