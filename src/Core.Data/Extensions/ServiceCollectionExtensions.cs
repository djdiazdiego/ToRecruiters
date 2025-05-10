using Core.Data.Repositories;
using Core.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Data.Extensions
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
    }
}
