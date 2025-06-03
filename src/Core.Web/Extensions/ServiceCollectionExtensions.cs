using Microsoft.Extensions.DependencyInjection;

namespace Core.Web.Extensions
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers global exception handler services, including ProblemDetails and a custom exception handler.
        /// </summary>
        /// <param name="services">The service collection to add the exception handler services to.</param>
        public static void AddGlobalExceptionHandlerServices(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
        }
    }
}
