using MediatR;

namespace Core.Head.CQRS
{
    /// <summary>
    /// Represents the base interface for all command types.
    /// </summary>
    public interface IBaseCommand { }

    /// <summary>
    /// Represents a command that returns a response of type <typeparamref name="TResponse"/>.
    /// Inherits from <see cref="IRequest{TResponse}"/> and <see cref="IBaseCommand"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
    public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
        where TResponse : class
    {
    }

    /// <summary>
    /// Represents a command that does not return a response.
    /// Inherits from <see cref="IRequest"/> and <see cref="IBaseCommand"/>.
    /// </summary>
    public interface ICommand : IRequest, IBaseCommand
    {
    }
}
