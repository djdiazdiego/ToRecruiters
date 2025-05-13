using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Core.Data.Factories
{
    /// <summary>
    /// Abstract factory class for creating instances of <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    public abstract class DbContextFactory<TContext> :
        IDesignTimeDbContextFactory<TContext>,
        IDbContextFactory<TContext>
        where TContext : DbContext
    {
        private readonly string? _connection;
        private readonly string _migrationsAssembly;
        private readonly DbTypes _dbType;
        private readonly IEnumerable<IInterceptor>? _interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory{TContext}"/> class.
        /// </summary>
        /// <param name="connectionString">The name of the connection string in the configuration.</param>
        /// <param name="migrationsAssembly">The name of the assembly containing migrations.</param>
        /// <param name="dbType">The type of the database.</param>
        protected DbContextFactory(string connectionString, string migrationsAssembly, DbTypes dbType)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            _connection = configuration.GetConnectionString(connectionString);

            EnsureConnectionString();

            _dbType = dbType;
            _migrationsAssembly = migrationsAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory{TContext}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="connectionString">The name of the connection string in the configuration.</param>
        /// <param name="migrationsAssembly">The name of the assembly containing migrations.</param>
        /// <param name="dbType">The type of the database.</param>
        /// <exception cref="ArgumentNullException">Thrown when the configuration object is null.</exception>
        protected DbContextFactory(IConfiguration configuration, string connectionString, string migrationsAssembly, DbTypes dbType)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            _connection = configuration.GetConnectionString(connectionString);

            EnsureConnectionString();

            _dbType = dbType;
            _migrationsAssembly = migrationsAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory{TContext}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration object used to retrieve settings.</param>
        /// <param name="interceptors">A collection of <see cref="IInterceptor"/> instances to be used by the DbContext.</param>
        /// <param name="connectionString">The key of the connection string in the configuration.</param>
        /// <param name="migrationsAssembly">The name of the assembly containing the EF Core migrations.</param>
        /// <param name="dbType">The type of the database to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="configuration"/> object is null.</exception>
        protected DbContextFactory(
            IConfiguration configuration,
            IEnumerable<IInterceptor>? interceptors,
            string connectionString,
            string migrationsAssembly,
            DbTypes dbType)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            _connection = configuration.GetConnectionString(connectionString);

            EnsureConnectionString();

            _dbType = dbType;
            _migrationsAssembly = migrationsAssembly;

            if (interceptors != null)
            {
                _interceptors = interceptors;
            }
        }

        /// <summary>
        /// Creates a new instance of the <typeparamref name="TContext"/> using the provided arguments.
        /// </summary>
        /// <param name="args">Optional arguments, such as a connection string.</param>
        /// <returns>An instance of <typeparamref name="TContext"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the connection string is null or empty.</exception>
        public TContext CreateDbContext(string[] args)
        {
            var connection = args?.FirstOrDefault() ?? _connection;

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentException("The database connection string cannot be null or empty.", nameof(args));
            }

            return Helpers.CreateDbContext<TContext>(connection, _migrationsAssembly, _dbType, interceptors: _interceptors);
        }

        /// <summary>
        /// Creates a new instance of the <typeparamref name="TContext"/> using the default connection string.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TContext"/>.</returns>
        public TContext CreateDbContext()
        {
            return CreateDbContext(Array.Empty<string>());
        }

        /// <summary>
        /// Ensures that the connection string is not null or empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the connection string is missing or empty in the configuration.
        /// </exception>
        private void EnsureConnectionString()
        {
            if (string.IsNullOrWhiteSpace(_connection))
            {
                throw new InvalidOperationException("The connection string is missing or empty in the configuration.");
            }
        }
    }
}
