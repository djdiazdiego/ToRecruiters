using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace PlayerHub.API.Filters
{
    public sealed class ApiKeyRequirement(string secret) : IAuthorizationRequirement
    {
        public const string Scheme = "ApiKey";
        public const string HeaderName = "x-api-key";

        public string Secret { get; } = secret;
    }

    public sealed class ApiKeyAuthorizationHandler(
        IHttpContextAccessor contextAccessor,
        IConfiguration configuration) : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
        private readonly string _apiKey = configuration[ApiKeyRequirement.Scheme] ??
            throw new UnauthorizedAccessException("Api key not found");

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApiKeyRequirement requirement)
        {
            var httpRequest = _contextAccessor.HttpContext!.Request;

            if (httpRequest is not null && httpRequest.Headers.TryGetValue(ApiKeyRequirement.HeaderName, out StringValues values)
                && values.Count == 1 && _apiKey.Equals(values[0]))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
