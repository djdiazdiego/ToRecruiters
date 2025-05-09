using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Core.Security
{
    /// <summary>
    /// Represents a requirement for API key-based authorization.
    /// </summary>
    /// <param name="secret">The secret key used for authorization.</param>
    public sealed class ApiKeyRequirement(string secret) : IAuthorizationRequirement
    {
        /// <summary>
        /// The scheme name for API key authentication.
        /// </summary>
        public const string Scheme = "ApiKey";

        /// <summary>
        /// The header name where the API key is expected.
        /// </summary>
        public const string HeaderName = "x-api-key";

        /// <summary>
        /// Gets the secret key used for authorization.
        /// </summary>
        public string Secret { get; } = secret;
    }

    /// <summary>
    /// Handles API key-based authorization by validating the provided API key.
    /// </summary>
    public sealed class ApiKeyAuthorizationHandler(
        IHttpContextAccessor contextAccessor,
        IConfiguration configuration) : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor ??
            throw new ArgumentNullException(nameof(contextAccessor));

        private readonly string _apiKey = configuration[ApiKeyRequirement.Scheme] ??
            throw new InvalidOperationException("API key configuration is missing.");

        /// <summary>
        /// Handles the authorization requirement by validating the API key in the request header.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The API key requirement to validate.</param>
        /// <returns>A completed task.</returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApiKeyRequirement requirement)
        {
            if (_contextAccessor.HttpContext?.Request is { } httpRequest &&
                httpRequest.Headers.TryGetValue(ApiKeyRequirement.HeaderName, out StringValues values) &&
                values.Count == 1 &&
                string.Equals(_apiKey, values[0], StringComparison.Ordinal))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
