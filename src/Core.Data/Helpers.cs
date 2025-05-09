using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    /// <summary>
    /// Provides helper methods for creating and configuring database contexts.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Creates a new instance of a database context with the specified configuration.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context to create.</typeparam>
        /// <param name="connection">The connection string to the database.</param>
        /// <param name="migrationsAssembly">The name of the assembly containing migrations.</param>
        /// <param name="dbType">The type of the database (e.g., SqlServer, PostgreSQL).</param>
        /// <returns>An instance of the specified database context type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the database type is not supported or the context instance could not be created.
        /// </exception>
        public static TContext CreateDbContext<TContext>(
            string connection,
            string migrationsAssembly,
            DbTypes dbType,
            DbContextOptionsBuilder? optionsBuilder = null) where TContext : DbContext
        {
            optionsBuilder ??= new DbContextOptionsBuilder<TContext>();

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

            var context = Activator.CreateInstance(typeof(TContext), optionsBuilder.Options) as TContext;

            return context ?? throw new InvalidOperationException($"Failed to create an instance of {typeof(TContext).Name}");
        }
    }
}
