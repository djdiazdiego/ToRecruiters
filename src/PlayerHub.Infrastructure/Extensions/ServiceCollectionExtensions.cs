using Core.Infrastructure.Extensions;
using Core.Infrastructure.Interceptors;
using IdentityAuthGuard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayerHub.Infrastructure.Contexts;
using System.Reflection;

namespace PlayerHub.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering infrastructure services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds infrastructure-related services, including DbContext factories and unit of work services, to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactoryServices(configuration);
            services.AddUnitOfWorkServices<ReadDbContext, WriteDbContext>([Assembly.GetExecutingAssembly()]);
        }

        /// <summary>
        /// Registers DbContext factory services and related dependencies in the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <exception cref="InvalidOperationException">Thrown if the connection string is missing or empty.</exception>
        private static void AddDbContextFactoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISaveChangesInterceptor, PublishDomainEventsInterceptor>();

            var connection = configuration.GetConnectionString(Constants.CONNECTION_STRING);

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("The connection string is missing or empty in the configuration.");
            }

            services.AddSingleton<IDbContextFactory<WriteDbContext>>(provider =>
            {
                var interceptors = provider.GetRequiredService<ISaveChangesInterceptor>();

                return new WriteDbContextFactory(configuration, [interceptors]);
            });

            services.AddSingleton<IDbContextFactory<ReadDbContext>>(new ReadDbContextFactory(configuration));

            services.AddIdentityAuthGuardDbContextFactoryServices(configuration);
        }
    }
}
