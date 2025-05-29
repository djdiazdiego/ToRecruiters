using Core.Data;
using Core.Head.Behaviors;
using Core.Head.Handlers;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace Core.Head.Extensions
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers FluentValidation validators from the specified assemblies.
        /// </summary>
        /// <param name="services">The service collection to add the validators to.</param>
        /// <param name="assemblies">The assemblies to scan for validators.</param>
        public static void AddFluentValidationServices(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Registers AutoMapper profiles from the specified assemblies.
        /// </summary>
        /// <param name="services">The service collection to add the AutoMapper profiles to.</param>
        /// <param name="assemblies">The assemblies to scan for AutoMapper profiles.</param>
        public static void AddAutoMapperServices(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                services.AddAutoMapper(assembly);
            }
        }

        /// <summary>
        /// Registers MediatR services and optionally adds validation and logging behaviors.
        /// </summary>
        /// <param name="services">The service collection to add the MediatR services to.</param>
        /// <param name="assemblies">The assemblies to scan for MediatR handlers.</param>
        /// <param name="validationBehavior">Indicates whether to add the validation behavior.</param>
        /// <param name="loggingBehavior">Indicates whether to add the logging behavior.</param>
        public static void AddMediatRServices(
            this IServiceCollection services,
            Assembly[] assemblies,
            bool validationBehavior = true,
            bool loggingBehavior = true)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assemblies);

                if (validationBehavior)
                {
                    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                }

                if (loggingBehavior)
                {
                    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                }
            });
        }

        /// <summary>
        /// Registers global exception handler services, including ProblemDetails and a custom exception handler.
        /// </summary>
        /// <param name="services">The service collection to add the exception handler services to.</param>
        public static void AddGlobalExceptionHandlerServices(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
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
