using Core.Infrastructure.Common;
using Core.Infrastructure.Factories;
using IdentityAuthGuard.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IdentityAuthGuard.Data.Contexts
{
    /// <summary>
    /// Factory class for creating instances of <see cref="AppDbContext"/>.
    /// Inherits functionality from <see cref="DbContextFactory{TContext}"/>.
    /// </summary>
    public sealed class AppDbContextFactory :
        DbContextFactory<AppDbContext>,
        IDesignTimeDbContextFactory<AppDbContext>,
        IDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// The connection string used to connect to the database.
        /// </summary>
        private const string ConnectionString = DatabaseConstants.CONNECTION_STRING;

        /// <summary>
        /// The name of the assembly containing database migrations.
        /// </summary>
        private const string MigrationsAssembly = DatabaseConstants.MIGRATIONS_ASSEMBLY;

        /// <summary>
        /// The type of the database being used (e.g., SQL Server, PostgreSQL).
        /// </summary>
        private const DbTypes DbType = DatabaseConstants.DB_TYPE;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContextFactory"/> class
        /// using a predefined connection string, migrations assembly, and database type.
        /// </summary>
        public AppDbContextFactory() : base(ConnectionString, MigrationsAssembly, DbType) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContextFactory"/> class
        /// using the provided configuration, connection string, migrations assembly, and database type.
        /// </summary>
        /// <param name="configuration">The configuration object containing application settings.</param>
        public AppDbContextFactory(IConfiguration configuration) :
            base(configuration, ConnectionString, MigrationsAssembly, DbType)
        { }
    }
}
