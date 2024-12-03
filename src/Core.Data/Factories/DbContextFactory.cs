using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core.Data.Factories
{
    public abstract class DbContextFactory<TContext> :
        IDesignTimeDbContextFactory<TContext>,
        IDbContextFactory<TContext>
        where TContext : DbContext
    {
        private readonly string _connection;
        private readonly string _migrationsAssembly;
        private readonly DbTypes _dbType;

        protected DbContextFactory(string migrationsAssembly, DbTypes dbType)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            _connection = configuration["ConnectionStrings:AppDbConnection"] ?? string.Empty;
            _migrationsAssembly = migrationsAssembly;
            _dbType = dbType;

            if (string.IsNullOrEmpty(_connection))
            {
                throw new InvalidOperationException("The connection string was not found in the environment variables");
            }
        }

        protected DbContextFactory(string migrationsAssembly, DbTypes dbType, IConfiguration configuration)
        {
            _connection = configuration["ConnectionStrings:AppDbConnection"] ?? string.Empty;
            _migrationsAssembly = migrationsAssembly;
            _dbType = dbType;

            if (string.IsNullOrEmpty(_connection))
            {
                throw new InvalidOperationException("The connection string was not found in the configuration collection");
            }
        }

        public TContext CreateDbContext(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = [_connection];
            }

            return Helpers.CreateDbContext<TContext>(
                args == null || args.Length == 0 ? _connection : args[0],
                _migrationsAssembly,
                _dbType);
        }

        public TContext CreateDbContext()
        {
            if (string.IsNullOrEmpty(_connection))
            {
                throw new Exception("The database connection string cannot be null or empty");
            }

            return CreateDbContext([_connection]);
        }
    }
}
