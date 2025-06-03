using Core.BaseModels;
using MediatR;
using System.Linq.Expressions;

namespace Core.Application.CQRS
{
    /// <summary>
    /// Defines a handler for processing queries of type <typeparamref name="TQuery"/> and returning a response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
    }

    /// <summary>
    /// Defines a handler for processing filterable queries of type <typeparamref name="TQuery"/> 
    /// and returning a response of type <typeparamref name="TResponse"/> for entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TQuery">The type of the query being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public interface IFilterQueryHandler<TEntity, TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TEntity : IEntity
        where TQuery : IQuery<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Builds a search expression for filtering entities of type <typeparamref name="TEntity"/> based on the provided search string.
        /// </summary>
        /// <param name="search">The search string used to filter entities.</param>
        /// <returns>An expression representing the filter criteria.</returns>
        Expression<Func<TEntity, bool>> BuildSearch(string search);
    }
}
