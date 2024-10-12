using Core.Data.Repositories;
using Core.BaseModels;

namespace Core.Data.UnitOfWorks
{
    public interface IWriteUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get write repository
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        WriteRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }
}
