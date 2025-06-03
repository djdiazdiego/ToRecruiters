using Core.BaseModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Infrastructure.Interceptors
{
    /// <summary>
    /// Interceptor to publish domain events before saving changes to the database.
    /// </summary>
    public class PublishDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
    {
        /// <summary>
        /// Synchronously handles the SavingChanges event to publish domain events.
        /// </summary>
        /// <param name="eventData">The event data containing the DbContext.</param>
        /// <param name="result">The interception result.</param>
        /// <returns>The interception result after processing.</returns>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = ValidateDbContext(eventData.Context);

            PublishDomainEventsAsync(context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Asynchronously handles the SavingChanges event to publish domain events.
        /// </summary>
        /// <param name="eventData">The event data containing the DbContext.</param>
        /// <param name="result">The interception result.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = ValidateDbContext(eventData.Context);

            await PublishDomainEventsAsync(context, cancellationToken).ConfigureAwait(false);
            return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes all domain events for tracked aggregates in the DbContext.
        /// </summary>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task PublishDomainEventsAsync(DbContext context, CancellationToken cancellationToken = default)
        {
            var aggregates = context.ChangeTracker.Entries<IAggregateRoot>()
                .Where(a => a.Entity.GetDomainEvents().Count != 0)
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a.GetDomainEvents())
                .ToList();

            aggregates.ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent, cancellationToken);
        }

        /// <summary>
        /// Validates that the provided DbContext is not null.
        /// </summary>
        /// <param name="context">The DbContext instance to validate.</param>
        /// <returns>The validated DbContext instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the DbContext is null.</exception>
        private static DbContext ValidateDbContext(DbContext? context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context), "DbContext cannot be null.");
            }

            return context;
        }
    }
}
