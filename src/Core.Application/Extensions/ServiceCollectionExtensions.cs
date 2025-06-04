using Core.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application.Extensions
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
    }
}
