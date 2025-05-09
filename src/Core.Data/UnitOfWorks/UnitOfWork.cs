using Core.BaseModels;
using Core.Data.Repositories;
using Core.DomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.UnitOfWorks
{
    /// <summary>
    /// Represents a unit of work for managing repositories and saving changes to the database.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    public sealed class UnitOfWork<TContext>(IDbContextFactory<TContext> factory, IMediator mediator, Type repositoryType) :
        IWriteUnitOfWork,
        IReadUnitOfWork,
        IDisposable
        where TContext : DbContext
    {
        private bool disposedValue;
        private readonly DbContext _context = factory.CreateDbContext();
        private readonly IMediator _mediator = mediator;
        private readonly Type _repositoryType = repositoryType;
        private readonly Dictionary<Type, IRepository> _repositories = [];

        /// <inheritdoc />
        WriteRepository<TEntity> IWriteUnitOfWork.GetRepository<TEntity>() =>
            GetRepository<WriteRepository<TEntity>>();

        /// <inheritdoc />
        ReadRepository<TEntity> IReadUnitOfWork.GetRepository<TEntity>() =>
            GetRepository<ReadRepository<TEntity>>();

        /// <inheritdoc />
        public int SaveChanges()
        {
            TryExecuteDomainEvents();
            return _context.SaveChanges();
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TryExecuteDomainEvents(cancellationToken);
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
        /// Executes domain events for the tracked entities.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        private void TryExecuteDomainEvents(CancellationToken cancellationToken = default)
        {
            List<IDomainEvent> events;
            do
            {
                events = GetEvents();
                if (events.Count > 0)
                {
                    PublishDomainEventsAsync(events, cancellationToken).GetAwaiter().GetResult();
                }
            } while (events.Count > 0);
        }

        /// <summary>
        /// Retrieves all domain events from the tracked entities.
        /// </summary>
        /// <returns>A list of domain events.</returns>
        private List<IDomainEvent> GetEvents()
        {
            var aggregates = _context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Modified or EntityState.Added or EntityState.Deleted &&
                            typeof(IAggregateRoot).IsAssignableFrom(e.Entity.GetType()))
                .Select(e => e.Entity)
                .Cast<IAggregateRoot>()
                .ToArray();

            var events = new List<IDomainEvent>();

            foreach (var aggregate in aggregates)
            {
                events.AddRange(aggregate.GetDomainEvents());
                aggregate.ClearDomainEvents();
            }

            return events;
        }

        /// <summary>
        /// Publishes the specified domain events asynchronously.
        /// </summary>
        /// <param name="events">The list of domain events to publish.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        private async Task PublishDomainEventsAsync(List<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            if (events == null || events.Count == 0)
            {
                return;
            }

            var publishTasks = events.Select(@event => _mediator.Publish(@event, cancellationToken));
            await Task.WhenAll(publishTasks);
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
