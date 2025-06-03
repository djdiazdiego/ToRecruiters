using Core.Application.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security;
using System.Security.Claims;

namespace Security
{
    /// <summary>
    /// Provides extension methods for configuring authentication and authorization services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures JWT-based authentication services.
        /// </summary>
        /// <param name="services">The service collection to add the authentication services to.</param>
        /// <param name="configuration">The application configuration containing JWT settings.</param>
        /// <exception cref="NotFoundException">Thrown when required JWT configuration values are missing.</exception>
        /// <exception cref="SecurityException">Thrown when JWT configuration values are invalid.</exception>
        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var issuer = configuration["Jwt:Issuer"]
                             ?? throw new NotFoundException("Jwt issuer not found in configuration.");
                var audience = configuration["Jwt:Audience"]
                               ?? throw new NotFoundException("Jwt audience not found in configuration.");
                var key = configuration["Jwt:Key"]
                          ?? throw new NotFoundException("Jwt key not found in configuration.");

                if (string.IsNullOrWhiteSpace(issuer))
                {
                    throw new SecurityException("JWT issuer cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(audience))
                {
                    throw new SecurityException("JWT audience cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new SecurityException("JWT key cannot be null or empty.");
                }

                options.TokenValidationParameters = Helpers.GetTokenValidationParameters(issuer, audience, key);
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
            });
        }

        /// <summary>
        /// Configures authorization services and policies.
        /// </summary>
        /// <param name="services">The service collection to add the authorization services to.</param>
        /// <param name="configuration">The application configuration containing API key settings.</param>
        /// <exception cref="InvalidOperationException">Thrown when API key configuration is missing.</exception>
        /// <exception cref="NotFoundException">Thrown when the API key is not found in the configuration.</exception>
        public static void AddAuthorizationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

            services.AddAuthorizationBuilder()
                .AddPolicy(Schemes.UserScheme, builder =>
                {
                    builder.RequireAuthenticatedUser()
                           .RequireClaim(ClaimTypes.NameIdentifier);
                })
                .AddPolicy(ApiKeyRequirement.Scheme, policy =>
                {
                    var secret = configuration[ApiKeyRequirement.Scheme]
                                 ?? throw new InvalidOperationException("API key configuration is missing.");

                    if (string.IsNullOrWhiteSpace(secret))
                    {
                        throw new NotFoundException("API key not found in configuration.");
                    }

                    policy.Requirements.Add(new ApiKeyRequirement(secret));
                });
        }
    }
}
