using MediatR;

namespace Core.Head.CQRS
{
    public interface IBaseCommand { }

    public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
        where TResponse : class
    {
    }

    public interface ICommand : IRequest, IBaseCommand
    {
    }
}
