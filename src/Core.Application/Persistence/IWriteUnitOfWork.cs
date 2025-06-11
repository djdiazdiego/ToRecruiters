using Core.BaseModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Persistence
{
    /// <summary>
    /// Represents a unit of work for write operations.
    /// </summary>
    public interface IWriteUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets a write repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A <see cref="IWriteRepository{TEntity}"/> instance.</returns>
        IWriteRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;

        /// <summary>
        /// Saves all changes made in this unit of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all changes made in this unit of work.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();
    }
}
