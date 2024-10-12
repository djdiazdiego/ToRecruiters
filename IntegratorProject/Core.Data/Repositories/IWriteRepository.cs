using Core.BaseModels;

namespace Core.Data.Repositories
{
    public interface IWriteRepository<TEntity> : IReadRepository<TEntity>, IRepository where TEntity : class, IEntity
    {
        /// <summary>
        /// Track the entity to be deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Remove(TEntity entity);

        /// <summary>
        /// Track the entities to be deleted
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(params TEntity[] entities);

        /// <summary>
        /// Track the entity to be updated
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Update(TEntity entity);

        /// <summary>
        /// Track the entities to be updated
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void UpdateRange(params TEntity[] entities);

        /// <summary>
        /// Track the entity to be inserted
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Add(TEntity entity);

        /// <summary>
        /// Track the entities to be updated
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(params TEntity[] entities);
    }
}
