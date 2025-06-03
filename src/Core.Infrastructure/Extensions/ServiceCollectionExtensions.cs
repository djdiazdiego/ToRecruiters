using Core.Application.Persistence;
using Core.Infrastructure.Repositories;
using Core.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering unit of work services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the unit of work services for the specified read and write DbContext types.
        /// </summary>
        /// <typeparam name="TReadDbContext">The type of the read DbContext.</typeparam>
        /// <typeparam name="TWriteDbContext">The type of the write DbContext.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        public static void AddUnitOfWorkServices<TReadDbContext, TWriteDbContext>(this IServiceCollection services)
            where TReadDbContext : DbContext
            where TWriteDbContext : DbContext
        {
            services.AddTransient<IReadUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<TReadDbContext>>();

                return new UnitOfWork<TReadDbContext>(factory, typeof(ReadRepository<>));
            });

            services.AddScoped<IWriteUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<TWriteDbContext>>();

                return new UnitOfWork<TWriteDbContext>(factory, typeof(WriteRepository<>));
            });
        }

        /// <summary>
        /// Registers health check services for the specified database type and connection string.
        /// </summary>
        /// <param name="services">The service collection to add the health checks to.</param>
        /// <param name="configuration">The application configuration containing connection strings.</param>
        /// <param name="connectionString">The name of the connection string in the configuration.</param>
        /// <param name="dbType">The type of database to check. Defaults to <see cref="DbTypes.SqlServer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the connection string is missing or empty.</exception>
        public static void AddHealthChecksServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionString,
            DbTypes dbType = DbTypes.SqlServer)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var connection = configuration.GetConnectionString(connectionString);

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("The connection string is missing or empty in the configuration.");
            }

            if (dbType == DbTypes.SqlServer)
            {
                services.AddHealthChecks()
                    .AddSqlServer(
                        connectionString: connection,
                        failureStatus: HealthStatus.Unhealthy,
                        tags: [nameof(DbTypes.SqlServer).ToLowerInvariant()]);
            }
            else if (dbType == DbTypes.PostgreSQL)
            {
                services.AddHealthChecks()
                    .AddNpgSql(
                        connectionString: connection,
                        failureStatus: HealthStatus.Unhealthy,
                        tags: [nameof(DbTypes.PostgreSQL).ToLowerInvariant()]);
            }
        }
    }
}
