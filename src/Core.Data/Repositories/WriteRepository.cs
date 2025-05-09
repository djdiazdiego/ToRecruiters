using Core.BaseModels;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Repositories
{
    /// <summary>
    /// A repository for performing write operations on entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public sealed class WriteRepository<TEntity>(DbContext dbContext) :
        ReadRepository<TEntity>(dbContext),
        IWriteRepository<TEntity> where TEntity : class, IEntity
    {
        /// <inheritdoc />
        public void Add(TEntity entity) => Context.Set<TEntity>().Add(entity);

        /// <inheritdoc />
        public void AddRange(params TEntity[] entities) => Context.Set<TEntity>().AddRange(entities);

        /// <inheritdoc />
        public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);

        /// <inheritdoc />
        public void RemoveRange(params TEntity[] entities) => Context.Set<TEntity>().RemoveRange(entities);

        /// <inheritdoc />
        public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);

        /// <inheritdoc />
        public void UpdateRange(params TEntity[] entities) => Context.Set<TEntity>().UpdateRange(entities);
    }
}
