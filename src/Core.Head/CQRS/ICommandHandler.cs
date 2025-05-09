using MediatR;

namespace Core.Head.CQRS
{
    /// <summary>
    /// Represents a handler for a command that produces a response.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response produced by the command.</typeparam>
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : class
    {
    }

    /// <summary>
    /// Represents a handler for a command that does not produce a response.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command being handled.</typeparam>
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }
}
