using Core.BaseModels;

namespace Core.Application.Persistence
{
    /// <summary>
    /// Represents a unit of work for read-only operations.
    /// </summary>
    public interface IReadUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets a read repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of <see cref="IReadRepository{TEntity}"/> for the specified entity type.</returns>
        IReadRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    }
}
