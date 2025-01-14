using Core.Data.Repositories;
using Core.Data.UnitOfWorks;
using Core.Head.Behaviors;
using Core.Head.Exceptions;
using Core.Head.Exceptions.Handlers;
using FluentValidation;
using IdentityAuthGuard;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Contracts;
using IdentityAuthGuard.Models;
using IdentityAuthGuard.Services.GuidGeneratorService;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PlayerHub.API.Filters;
using PlayerHub.Data;
using PlayerHub.Data.Contexts;
using System.Reflection;
using System.Security.Claims;

namespace PlayerHub.API.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private const string APPLICATION_ASSEMBLY = "PlayerHub.Application";

        public static void AddAPIServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddCorsServices(builder.Environment);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllerServices();
            builder.Services.AddRouting(x => x.LowercaseUrls = true);
            builder.Services.AddGlobalExceptionHandlerServices();
            builder.Services.AddSwaggerServices(builder.Environment);
        }
        public static void AddDataServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextFactoryServices(builder.Configuration);
            builder.Services.AddUnitOfWorkServices();
        }
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationServices();
            builder.Services.AddAutoMapperServices();
            builder.Services.AddMediatRServices();
        }
        public static void AddIdentityAuthGuardServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddIdentityServices();
            builder.Services.AddUserServices();
        }
        public static void AddAPISecurityServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddAuthenticationServices(builder.Configuration);
            builder.Services.AddAuthorizationServices(builder.Configuration);
        }

        #region API Services
        private static void AddCorsServices(this IServiceCollection services, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(
                        name: "AllowOrigin", config =>
                        {
                            config.WithOrigins("http://localhost:4200")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                });
            }
        }
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
        private static void AddGlobalExceptionHandlerServices(this IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
        }
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
        #endregion

        #region Data Services
        private static void AddDbContextFactoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<WriteDbContext, WriteDbContextFactory>();
            services.AddDbContextFactory<ReadDbContext, ReadDbContextFactory>();
            services.AddDbContextFactory<AppDbContext, AppDbContextFactory>(options =>
            {
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif

                var connection = configuration["ConnectionStrings:AppDbConnection"];

                options.UseSqlServer(connection, o =>
                {
                    o.MigrationsAssembly(Constants.MIGRATIONS_ASSEMBLY);
                    o.EnableRetryOnFailure();
                });
            });
        }
        private static void AddUnitOfWorkServices(this IServiceCollection services)
        {
            services.AddTransient<IReadUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<ReadDbContext>>();
                var mediator = provider.GetRequiredService<IMediator>();
                var repositoryType = typeof(ReadRepository<>);

                return new UnitOfWork<ReadDbContext>(factory, mediator, repositoryType);
            });

            services.AddScoped<IWriteUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<WriteDbContext>>();
                var mediator = provider.GetRequiredService<IMediator>();
                var repositoryType = typeof(WriteRepository<>);

                return new UnitOfWork<WriteDbContext>(factory, mediator, repositoryType);
            });
        }
        #endregion

        #region Application Services
        private static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.Load(APPLICATION_ASSEMBLY));
        }
        private static void AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.Load(APPLICATION_ASSEMBLY));
        }
        private static void AddMediatRServices(this IServiceCollection services)
        {
            var assemblies = new Assembly[] { Assembly.Load(APPLICATION_ASSEMBLY) };

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assemblies);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
        }
        #endregion

        #region Identity Auth Guard Services
        private static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddRoles<Role>()
                .AddSignInManager();
        }
        private static void AddUserServices(this IServiceCollection services)
        {
            services.AddSingleton<IGuidGeneratorService, GuidGeneratorService>();
            services.AddScoped<IUserService, UserService>();
        }
        #endregion

        #region API Security Services
        private static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var issuer = configuration["Jwt:Issuer"] ?? throw new NotFoundException("Jwt issuer not found");
                var audience = configuration["Jwt:Audience"] ?? throw new NotFoundException("Jwt audience not found");
                var key = configuration["Jwt:Key"] ?? throw new NotFoundException("Jwt key not found");

                options.TokenValidationParameters = Helpers.GetTokenValidationParameters(issuer, audience, key);
            });
        }
        private static void AddAuthorizationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

            services.AddAuthorizationBuilder()
                .AddPolicy(Schemes.UserScheme, builder =>
                {
                    builder.RequireAuthenticatedUser().RequireClaim(ClaimTypes.NameIdentifier);
                })
                .AddPolicy(ApiKeyRequirement.Scheme, policy =>
                {
                    var secret = configuration[ApiKeyRequirement.Scheme] ??
                        throw new NotFoundException("Api key not found");

                    policy.Requirements.Add(new ApiKeyRequirement(secret));
                });
        }
        #endregion
    }
}
