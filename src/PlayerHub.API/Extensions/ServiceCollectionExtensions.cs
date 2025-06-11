using Microsoft.OpenApi.Models;
using System.Reflection;

namespace PlayerHub.API.Extensions
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        public const string PLAYER_HUB_CORS_POLICY = "PlayerHubCorsPolicy";

        /// <summary>
        /// Configures and adds API-related services to the specified service collection.   
        /// </summary>
        /// <remarks>This method adds services required for API functionality, including CORS and Swagger
        /// support. The configuration of these services may vary depending on the provided hosting
        /// environment.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the API services will be added.</param>
        /// <param name="environment">The <see cref="IHostEnvironment"/> representing the current hosting environment. Used to configure services
        /// based on the environment.</param>
        public static void AddAPIServices(this IServiceCollection services, IHostEnvironment environment)
        {
            services.AddCorsServices(environment);
            services.AddSwaggerServices(environment);
        }

        /// <summary>
        /// Configures CORS policies for the application.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="environment">The hosting environment.</param>
        private static void AddCorsServices(this IServiceCollection services, IHostEnvironment environment)
        {
            services.AddCors(options =>
                {
                    options.AddPolicy(PLAYER_HUB_CORS_POLICY, corsPolicy =>
                    {
                        corsPolicy.WithHeaders("Content-Type", "Authorization")
                                .WithMethods("GET", "POST", "PUT", "DELETE")
                                .AllowCredentials();

                        if (environment.IsDevelopment())
                        {
                            corsPolicy.WithOrigins("http://localhost:4200");
                        }
                        else
                        {
                            corsPolicy.WithOrigins("https://playerhub.dev")
                                    .SetIsOriginAllowedToAllowWildcardSubdomains();
                        }
                    });
                });
        }

        /// <summary>
        /// Configures Swagger services for API documentation.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="environment">The hosting environment.</param>
        private static void AddSwaggerServices(this IServiceCollection services, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.CustomSchemaIds(t => t.FullName);

                    options.UseInlineDefinitionsForEnums();
                    options.DescribeAllParametersInCamelCase();
                    options.UseAllOfToExtendReferenceSchemas();

                    string[] versions = ["v1"];

                    foreach (var version in versions)
                    {
                        options.SwaggerDoc(version, new OpenApiInfo
                        {
                            Title = "PlayerHub.API",
                            Version = version,
                            Description = "API Documentation",
                            Contact = new OpenApiContact
                            {
                                Name = "Dayron Jesus Diaz Diego",
                                Email = "dj.diazdiego@gmail.com"
                            }
                        });
                    }

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Description = "Bearer Token",
                        Scheme = "Bearer"
                    });

                    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                    {
                        Name = "x-api-key",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Description = "Api Key",
                        Scheme = "ApiKeyScheme"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer",
                                    },
                                    Scheme = "oauth2",
                                    Name = "Bearer",
                                    In = ParameterLocation.Header,
                                }, []
                            },
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "ApiKey"
                                    }
                                }, []
                            },
                    });

                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                });

                services.AddSwaggerGenNewtonsoftSupport();
            }
        }
    }
}
