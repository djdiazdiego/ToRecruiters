using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Data.Helpers
{
    /// <summary>
    /// Provides helper methods for creating and configuring DbContext instances.
    /// </summary>
    public static class DbContextHelpers
    {
        /// <summary>
        /// Creates a new instance of the specified DbContext type.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext to create.</typeparam>
        /// <param name="connection">The connection string to the database.</param>
        /// <param name="migrationsAssembly">The assembly containing the migrations.</param>
        /// <param name="dbType">The type of the database (e.g., SQL Server, PostgreSQL).</param>
        /// <param name="optionsBuilder">Optional DbContextOptionsBuilder to configure the DbContext.</param>
        /// <param name="interceptors">Optional collection of interceptors to add to the DbContext.</param>
        /// <returns>An instance of the specified DbContext type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the DbContext instance cannot be created.</exception>
        public static TContext CreateDbContext<TContext>(
           string connection,
           string migrationsAssembly,
           DbTypes dbType,
           DbContextOptionsBuilder? optionsBuilder = null,
           IEnumerable<IInterceptor>? interceptors = null) where TContext : DbContext
        {
            optionsBuilder ??= new DbContextOptionsBuilder<TContext>();

            ConfigureDbContextOptions<TContext>(connection, migrationsAssembly, dbType, optionsBuilder, interceptors);

            return CreateDbContext<TContext>(optionsBuilder);
        }

        /// <summary>
        /// Configures the DbContextOptions for the specified DbContext type.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext to configure.</typeparam>
        /// <param name="connection">The connection string to the database.</param>
        /// <param name="migrationsAssembly">The assembly containing the migrations.</param>
        /// <param name="dbType">The type of the database (e.g., SQL Server, PostgreSQL).</param>
        /// <param name="optionsBuilder">The DbContextOptionsBuilder to configure.</param>
        /// <param name="interceptors">Optional collection of interceptors to add to the DbContext.</param>
        /// <exception cref="InvalidOperationException">Thrown if the database type is unsupported.</exception>
        public static void ConfigureDbContextOptions<TContext>(
            string connection,
            string migrationsAssembly,
            DbTypes dbType,
            DbContextOptionsBuilder optionsBuilder,
            IEnumerable<IInterceptor>? interceptors = null) where TContext : DbContext
        {
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif

            switch (dbType)
            {
                case DbTypes.SqlServer:
                    optionsBuilder.UseSqlServer(connection, o =>
                    {
                        o.MigrationsAssembly(migrationsAssembly);
                        o.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
                    break;
                case DbTypes.PostgreSQL:
                    optionsBuilder.UseNpgsql(connection, o =>
                    {
                        o.MigrationsAssembly(migrationsAssembly);
                        o.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported database type: {dbType}");
            }

            if (interceptors?.Any() == true)
            {
                optionsBuilder.AddInterceptors(interceptors);
            }
        }

        /// <summary>
        /// Creates a new instance of the specified DbContext type using the provided options.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext to create.</typeparam>
        /// <param name="optionsBuilder">The DbContextOptionsBuilder containing the configuration.</param>
        /// <returns>An instance of the specified DbContext type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the DbContext instance cannot be created.</exception>
        private static TContext CreateDbContext<TContext>(DbContextOptionsBuilder optionsBuilder)
            where TContext : DbContext
        {
            var context = Activator.CreateInstance(typeof(TContext), optionsBuilder.Options) as TContext;
            return context ?? throw new InvalidOperationException($"Failed to create an instance of {typeof(TContext).Name}");
        }
    }
}
