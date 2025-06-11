using Core.Application.Exceptions;
using Core.Infrastructure.Common;
using Core.Infrastructure.Telemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Trace;

namespace Core.Web.Extensions
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers web-related services, including logging, routing, controllers, exception handling, HSTS, rate limiting, and OpenTelemetry.
        /// </summary>
        /// <param name="services">The service collection to add the exception handler services to.</param>
        /// <returns></returns>
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddLogging(); // Logging comes first for diagnostics.
            services.AddHttpContextAccessor(); // Provides access to the current HTTP context.
            services.AddRouting(x => x.LowercaseUrls = true); // Configures routing behavior before controllers.
            services.AddControllerServices(); // Adds controllers and API services.
            services.AddGlobalExceptionHandlerServices(); // Registers global exception handling early.
            services.AddHstsServices(); // Configures HSTS (HTTP Strict Transport Security).
            services.AddRateLimiterServices(); // Enables rate limiting to control API traffic.
            services.AddOpenTelemetryServices(); // Registers OpenTelemetry for distributed tracing.

            return services;
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
        /// Registers OpenTelemetry tracing services, including sources, samplers, and instrumentations.
        /// </summary>
        /// <param name="services">The service collection to add the OpenTelemetry services to.</param>
        public static void AddOpenTelemetryServices(this IServiceCollection services)
        {
            services.AddSingleton<IActivityTelemetryService, ActivityTelemetryService>();

            var telemetryActivityNames = new string[]
            {
                    TelemetryActivityNames.DATABASE_ACCESS,
                    TelemetryActivityNames.HTTP_REQUEST,
                    TelemetryActivityNames.HTTP_RESPONSE,
                    TelemetryActivityNames.INTEGRATION_EVENT
            };

            services.AddOpenTelemetry().WithTracing(providerBuilder =>
            {
                providerBuilder.AddSource(telemetryActivityNames)
                    .SetSampler(new AlwaysOnSampler())
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddConsoleExporter();
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
