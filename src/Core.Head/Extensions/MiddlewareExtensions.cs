using Core.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Core.Head.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring middleware in the application.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds health check endpoints to the application's endpoint routing.
        /// </summary>
        /// <param name="app">The endpoint route builder to add the health check endpoints to.</param>
        /// <param name="dbTypes">The type of database to filter health checks for the database-specific endpoint.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported <paramref name="dbTypes"/> value is provided.</exception>
        public static void MapHealthChecks(
            this IEndpointRouteBuilder app,
            DbTypes dbTypes)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).RequireAuthorization();

            var dbName = dbTypes switch
            {
                DbTypes.SqlServer => nameof(DbTypes.SqlServer).ToLowerInvariant(),
                DbTypes.PostgreSQL => nameof(DbTypes.PostgreSQL).ToLowerInvariant(),
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(dbName))
            {
                app.MapHealthChecks("/health/db", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains(dbName),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).RequireAuthorization();
            }
        }
    }
}
