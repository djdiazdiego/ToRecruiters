using Core.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PlayerHub.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application-specific services into the dependency injection container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        public static void AddApplicationServices(this IServiceCollection services)
        {
            /// <summary>
            /// Registers FluentValidation services for the application.
            /// </summary>
            /// <remarks>
            /// Scans the executing assembly for validators and adds them to the service collection.
            /// </remarks>
            services.AddFluentValidationServices([Assembly.GetExecutingAssembly()]);

            /// <summary>
            /// Registers AutoMapper services for the application.
            /// </summary>
            /// <remarks>
            /// Scans the executing assembly for mapping profiles and adds them to the service collection.
            /// </remarks>
            services.AddAutoMapperServices([Assembly.GetExecutingAssembly()]);

            /// <summary>
            /// Registers MediatR services for the application.
            /// </summary>
            /// <remarks>
            /// Scans the executing assembly for MediatR handlers and adds them to the service collection.
            /// </remarks>
            services.AddMediatRServices([Assembly.GetExecutingAssembly()]);
        }
    }
}
