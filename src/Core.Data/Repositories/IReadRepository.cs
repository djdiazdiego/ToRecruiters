using Core.BaseModels;

namespace Core.Data.Repositories
{
    /// <summary>
    /// Represents a read-only repository for accessing entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IReadRepository<TEntity> : IRepository where TEntity : class, IEntity
    {
        /// <inheritdoc cref="IRepository"/>
        /// <summary>
        /// Gets a queryable collection of the entity.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> for querying the entity.</returns>
        IQueryable<TEntity> GetQuery();

        /// <summary>
        /// Finds an entity asynchronously by its primary keys.
        /// </summary>
        /// <param name="keyValues">The primary key values of the entity.</param>
        /// <param name="cancelationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="ValueTask{TEntity}"/> representing the asynchronous operation. The task result contains the entity if found; otherwise, <c>null</c>.</returns>
        ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancelationToken = default);

        /// <summary>
        /// Finds an entity by its primary keys.
        /// </summary>
        /// <param name="keyValues">The primary key values of the entity.</param>
        /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
        TEntity? Find(object[] keyValues);
    }
}
