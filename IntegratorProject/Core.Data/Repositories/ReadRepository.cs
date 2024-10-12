using Core.BaseModels;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Repositories
{
    public class ReadRepository<TEntity>(DbContext dbContext) : IReadRepository<TEntity>, IRepository where TEntity : class, IEntity
    {
        private readonly DbContext _context = dbContext;

        protected DbContext Context => _context;

        /// <inheritdoc />
        public async ValueTask<TEntity?> FindAsync(object[] keyValues, CancellationToken cancelationToken = default) =>
            await _context.Set<TEntity>().FindAsync(keyValues, cancelationToken);

        /// <inheritdoc />
        public TEntity? Find(object[] keyValues) => _context.Set<TEntity>().Find(keyValues);

        /// <inheritdoc />
        public IQueryable<TEntity> GetQuery() => _context.Set<TEntity>();
    }
}
