using Core.BaseModels;

namespace Core.Data.Repositories
{
    public interface IReadRepository<TEntity> : IRepository where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets a query of the entity
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetQuery();

        /// <summary>
        /// Get an entity by its primary keys
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="cancelationToken"></param>
        /// <returns></returns>
        ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancelationToken = default);

        /// <summary>
        /// Get an entity by its primary keys
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity? Find(object[] keyValues);
    }
}
