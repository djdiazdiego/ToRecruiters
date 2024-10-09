using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features
{
    public abstract class CommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : Command<TResponse>
        where TResponse : class
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }

    public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand>
        where TCommand : Command
    {
        public abstract Task Handle(TCommand request, CancellationToken cancellationToken);
    }
}
