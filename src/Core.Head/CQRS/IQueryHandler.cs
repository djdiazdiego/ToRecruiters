using Core.BaseModels;
using MediatR;
using System.Linq.Expressions;

namespace Core.Head.CQRS
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
    }

    public interface IFilterQueryHandler<TEntity,
        TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TEntity : IEntity
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
        Expression<Func<TEntity, bool>> BuildSearch(string search);
    }
}
