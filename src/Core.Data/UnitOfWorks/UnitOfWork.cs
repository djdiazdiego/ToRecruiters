using Core.BaseModels;
using Core.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.UnitOfWorks
{
    /// <summary>
    /// Represents a unit of work that provides access to repositories and manages database transactions.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="factory">The factory used to create instances of the database context.</param>
    /// <param name="repositoryType">The type of the repository to be used.</param>
    public sealed class UnitOfWork<TContext>(IDbContextFactory<TContext> factory, Type repositoryType) :
        IWriteUnitOfWork,
        IReadUnitOfWork,
        IDisposable
        where TContext : DbContext
    {
        private bool disposedValue;
        private readonly DbContext _context = factory.CreateDbContext();
        private readonly Type _repositoryType = repositoryType;
        private readonly Dictionary<Type, IRepository> _repositories = [];

        /// <inheritdoc />
        IWriteRepository<TEntity> IWriteUnitOfWork.GetRepository<TEntity>() =>
            GetRepository<WriteRepository<TEntity>>();

        /// <inheritdoc />
        IReadRepository<TEntity> IReadUnitOfWork.GetRepository<TEntity>() =>
            GetRepository<ReadRepository<TEntity>>();

        /// <inheritdoc />
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a repository of the specified type.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns>An instance of the specified repository type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the repository type is incorrect or cannot be created.</exception>
        private TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        {
            var repositoryType = typeof(TRepository);

            if (!repositoryType.IsGenericType || repositoryType.GetGenericTypeDefinition() != _repositoryType)
            {
                throw new InvalidOperationException($"Incorrect repository type: {repositoryType.Name}");
            }

            var entityType = repositoryType.GetGenericArguments().First();

            if (_repositories.TryGetValue(entityType, out IRepository? existingRepository))
            {
                return (TRepository)existingRepository;
            }

            if (!entityType.IsClass || entityType.IsAbstract || !typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new InvalidOperationException($"Cannot create a repository for the type: {entityType.Name}");
            }

            var genericRepositoryType = _repositoryType.MakeGenericType(entityType);
            if (Activator.CreateInstance(genericRepositoryType, _context) is not TRepository newRepository)
            {
                throw new InvalidOperationException($"Failed to create an instance of repository type: {genericRepositoryType.Name}");
            }

            _repositories[entityType] = newRepository;
            return newRepository;
        }

        /// <summary>
        /// Disposes the resources used by the unit of work.
        /// </summary>
        /// <param name="disposing">Indicates whether the method is called from the Dispose method.</param>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
