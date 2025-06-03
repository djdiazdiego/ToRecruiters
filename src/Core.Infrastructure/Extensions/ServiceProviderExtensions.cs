using Core.Extensions;
using Core.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for applying database migrations and seeding data.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Applies any pending migrations for the specified database contexts.
        /// </summary>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="contextTypes">An array of database context types to apply migrations to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the database context instance cannot be resolved.</exception>
        public static async Task ApplyPenndingMigrationAsync(this IServiceProvider serviceProvider, Type[] contextTypes)
        {
            using var source = new CancellationTokenSource();

            MethodInfo? methodInfo = typeof(ServiceProviderExtensions).GetMethod(
                nameof(ApplyMigrationAsync),
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            foreach (var contextType in contextTypes)
            {
                if (methodInfo?.MakeGenericMethod(contextType).Invoke(null, [serviceProvider, source.Token]) is Task task)
                {
                    await task;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to invoke ApplyPenndingMigrationAsync for context type {contextType.FullName}.");
                }
            }
        }

        /// <summary>
        /// Applies seed data to the database using the specified assemblies.
        /// </summary>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="assemblies">An array of assemblies containing seed data implementations.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ApplySeedAsync(this IServiceProvider serviceProvider, Assembly[] assemblies)
        {
            var types = typeof(ISeed).GetConcreteTypes(assemblies: assemblies);

            using var source = new CancellationTokenSource();

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is ISeed seed)
                {
                    await seed.SeedAsync(serviceProvider, source.Token);
                }
            }
        }

        
        /// <summary>
        /// Applies any pending migrations for the specified database context.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context.</typeparam>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="IDbContextFactory{TContext}"/> cannot be resolved.
        /// </exception>
        private static async Task ApplyMigrationAsync<TContext>(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default) where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();

            var factory = scope.ServiceProvider.GetService<IDbContextFactory<TContext>>() ??
                throw new InvalidOperationException($"Failed to resolve {nameof(IDbContextFactory<TContext>)} for {typeof(TContext).FullName}.");

            using var context = factory.CreateDbContext();

            if (!string.Equals(context.Database.ProviderName, "Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase) &&
                (await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await context.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
