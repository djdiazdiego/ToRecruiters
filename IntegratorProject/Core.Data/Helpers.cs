using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public static class Helpers
    {
        public static TContext CreateDbContext<TContext>(
            string connection,
            string migrationsAssembly,
            DbTypes dbType) where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

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
                        o.EnableRetryOnFailure();
                    });
                    break;
                case DbTypes.PostgreSQL:
                    optionsBuilder.UseNpgsql(connection, o =>
                    {
                        o.MigrationsAssembly(migrationsAssembly);
                        o.EnableRetryOnFailure();
                    });
                    break;
                default:
                    throw new InvalidOperationException("The instance of TContext could not be created.");
            }

            var context = (TContext?)Activator.CreateInstance(typeof(TContext), [optionsBuilder.Options]);

            return context ?? throw new InvalidOperationException("The instance of TContext could not be created.");
        }
    }
}
