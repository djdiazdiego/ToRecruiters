using Core.BaseModels;

namespace Core.Application.Persistence
{
    /// <summary>
    /// Represents a repository for performing write operations on entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IWriteRepository<TEntity> : IReadRepository<TEntity>, IRepository where TEntity : class, IEntity
    {
        /// <summary>
        /// Removes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Removes the specified entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        void RemoveRange(params TEntity[] entities);

        /// <summary>
        /// Updates the specified entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified entities in the repository.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        void UpdateRange(params TEntity[] entities);

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds the specified entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        void AddRange(params TEntity[] entities);
    }
}
