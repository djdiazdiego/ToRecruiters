using Core.Application.Persistence;
using Core.BaseModels;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Core.Infrastructure.UnitOfWorks
{
    /// <summary>
    /// Represents a unit of work that provides access to repositories and manages database transactions.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="factory">The factory used to create instances of the database context.</param>
    /// <param name="repositoryType">The type of the repository to be used.</param>
    /// <param name="repositoryAssemblies">The assemblies where repository implementations are located.</param>
    public sealed class UnitOfWork<TContext>(IDbContextFactory<TContext> factory, Type repositoryType, params Assembly[] repositoryAssemblies) :
        IWriteUnitOfWork,
        IReadUnitOfWork,
        IDisposable
        where TContext : DbContext
    {
        private bool disposedValue;
        private readonly DbContext _context = factory.CreateDbContext();
        private readonly Type _repositoryType = repositoryType;
        private readonly Assembly[] _repositoryAssemblies = repositoryAssemblies;
        private readonly Dictionary<Type, IRepository> _repositories = [];

        /// <inheritdoc />
        IWriteRepository<TEntity> IWriteUnitOfWork.GetRepository<TEntity>() =>
            GetValidRepository<TEntity, IWriteRepository<TEntity>>();

        /// <inheritdoc />
        IReadRepository<TEntity> IReadUnitOfWork.GetRepository<TEntity>() =>
            GetValidRepository<TEntity, IReadRepository<TEntity>>();

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
        /// Retrieves a repository of the specified type for a specific entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of the specified repository type for the given entity type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the repository type is incorrect or cannot be created.</exception>
        private IRepository GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (_repositoryAssemblies is null || _repositoryAssemblies.Length == 0)
            {
                throw new InvalidOperationException("Repository assemblies are not provided or empty.");
            }

            var entityType = typeof(TEntity);

            if (entityType.IsAbstract)
            {
                throw new InvalidOperationException($"Cannot create a repository for the type: {entityType.Name}");
            }

            if (_repositories.TryGetValue(entityType, out IRepository? existingRepository))
            {
                if (existingRepository is null)
                {
                    throw new InvalidOperationException($"Repository for type {entityType.Name} is not initialized.");
                }

                return existingRepository;
            }

            var genericRepositoryType = _repositoryType.MakeGenericType(entityType);

            var repositories = genericRepositoryType.GetConcreteTypes(
                filter: t => t.BaseType == genericRepositoryType,
                assemblies: _repositoryAssemblies) ?? [];

            if (repositories.Length == 0)
            {
                throw new InvalidOperationException($"No concrete repository found for type: {genericRepositoryType.Name} in the provided assemblies.");
            }

            var repositoryConcreteType = repositories[0];

            if (Activator.CreateInstance(repositoryConcreteType, _context) is not IRepository newRepository)
            {
                throw new InvalidOperationException($"Failed to create an instance of repository type: {genericRepositoryType.Name}");
            }

            _repositories[entityType] = newRepository;

            return newRepository;
        }

        /// <summary>
        /// Retrieves a repository of the specified type for a specific entity type and ensures it is of the expected repository interface.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TRepository">The expected repository interface type.</typeparam>
        /// <returns>An instance of the specified repository type for the given entity type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the repository is not of the expected type.</exception>
        private TRepository GetValidRepository<TEntity, TRepository>()
            where TEntity : class, IEntity
            where TRepository : IRepository
        {
            var repository = GetRepository<TEntity>();

            if (repository is not TRepository validRepository)
            {
                throw new InvalidOperationException($"Repository for type {typeof(TEntity).Name} is not a valid repository.");
            }

            return validRepository;
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
