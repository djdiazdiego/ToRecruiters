using Core.Exceptions;
using Core.Head.Exceptions.Handlers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace PlayerHub.API.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring services in the application.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        public const string PLAYER_HUB_CORS_POLICY = "PlayerHubCorsPolicy";

        /// <summary>
        /// Adds API-related services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="environment">The hosting environment.</param>
        public static void AddAPIServices(this IServiceCollection services, IHostEnvironment environment)
        {
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddControllerServices();
            services.AddHstsServices();
            services.AddCorsServices(environment);
            services.AddRateLimiterServices();
            services.AddRouting(x => x.LowercaseUrls = true);
            services.AddGlobalExceptionHandlerServices();
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
        /// Configures controller-related services, including JSON serialization and OData.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private static void AddControllerServices(this IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ActionContext>>();

                        var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                            .SelectMany(v => v.Errors)
                            .Select(v => v.ErrorMessage.Trim([' ', '.']));

                        string detail = string.Join(". ", errors);

                        logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}", detail, DateTime.UtcNow);

                        var problemDetails = new ProblemDetails
                        {
                            Title = nameof(BadRequestException),
                            Detail = detail,
                            Status = StatusCodes.Status400BadRequest,
                            Instance = context.HttpContext.Request.Path
                        };

                        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        problemDetails.Extensions.Add("ValidationErrors", errors);

                        var result = new BadRequestObjectResult(problemDetails);

                        result.ContentTypes.Add("application/json");

                        return result;
                    };
                }).AddOData(options =>
                {
                    options.Select()
                        .Filter()
                        .OrderBy()
                        .Count()
                        .SetMaxTop(5000)
                        .Expand();
                });
        }

        /// <summary>
        /// Configures global exception handling services.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private static void AddGlobalExceptionHandlerServices(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
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

        /// <summary>
        /// Configures rate limiting services.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private static void AddRateLimiterServices(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", limiterOptions =>
                {
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.PermitLimit = 100;
                });
            });
        }

        /// <summary>
        /// Configures HSTS (HTTP Strict Transport Security) services.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private static void AddHstsServices(this IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
        }
    }
}
