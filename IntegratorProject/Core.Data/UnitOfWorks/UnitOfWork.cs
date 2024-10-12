using Core.BaseModels;
using Core.Data.Repositories;
using Core.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.UnitOfWorks
{
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

        WriteRepository<TEntity> IWriteUnitOfWork.GetRepository<TEntity>() =>
            GetRepository<WriteRepository<TEntity>>();

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

        private TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        {
            var repositoryType = typeof(TRepository);

            if (!repositoryType.IsGenericType || repositoryType.GetGenericTypeDefinition() != _repositoryType)
            {
                throw new Exception($"Incorrect repository type: {repositoryType.Name}");
            }

            var entityType = typeof(TRepository).GetGenericArguments().First();

            if (_repositories.TryGetValue(entityType, out IRepository? value))
            {
                return (TRepository)value;
            }

            var type = typeof(IEntity);

            if (entityType.IsClass && !entityType.IsAbstract && type.IsAssignableFrom(entityType))
            {
                var genericType = _repositoryType.MakeGenericType(entityType);
                var instance = Activator.CreateInstance(genericType, _context);

                if (instance is TRepository repository)
                {
                    _repositories.Add(entityType, repository);
                }

                return (TRepository)_repositories[entityType];
            }
            else
            {
                throw new Exception($"Cannot create a repository of the following type: {entityType.Name}");
            }
        }

        private void TryExecuteDomainEvents(CancellationToken cancellationToken = default)
        {
            var events = GetEvents();

            while (events.Count > 0)
            {
                PublishDomainEventsAsync(events, cancellationToken).Wait(cancellationToken);

                events = GetEvents();
            }
        }

        private List<Event> GetEvents()
        {
            var aggregates = _context.ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted) &&
                    e.GetType().GetInterfaces().Contains(typeof(IAggregateRoot)))
                .Select(e => e.Entity)
                .Cast<IAggregateRoot>()
                .ToArray();

            var events = new List<Event>();

            if (aggregates != null && aggregates.Length != 0)
            {
                foreach (var aggregate in aggregates)
                {
                    events.AddRange(aggregate.GetDomainEvents());

                    aggregate.ClearDomainEvents();
                }
            }

            return events;
        }

        private async Task PublishDomainEventsAsync(List<Event> events, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();

            foreach (var @event in events)
            {
                tasks.Add(_mediator.Publish(@event, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

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
