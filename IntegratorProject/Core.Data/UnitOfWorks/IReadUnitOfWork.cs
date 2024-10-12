using Core.BaseModels;
using Core.Data.Repositories;

namespace Core.Data.UnitOfWorks
{
    public interface IReadUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get read repository
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        ReadRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    }
}
