using Core.Data.Repositories;
using Core.Data.UnitOfWorks;
using Core.Head.Behaviors;
using Core.Head.Exceptions;
using Core.Head.Exceptions.Handlers;
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

        public static void AddLoggingServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddLogging();
        }

        public static void AddCorsServices(this IHostApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddCors(options =>
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

        public static void AddHttpContextAccessorServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
        }

        public static void AddControllerServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddControllers()
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
                }); ;
        }

        public static void AddDbContextFactoryServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextFactory<WriteDbContext, WriteDbContextFactory>();
            builder.Services.AddDbContextFactory<ReadDbContext, ReadDbContextFactory>();
            builder.Services.AddDbContextFactory<AppDbContext, AppDbContextFactory>(options =>
            {
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif

                var connection = builder.Configuration["ConnectionStrings:AppDbConnection"];

                options.UseSqlServer(connection, o =>
                {
                    o.MigrationsAssembly(Constants.MIGRATIONS_ASSEMBLY);
                    o.EnableRetryOnFailure();
                });
            });
        }

        public static void AddUnitOfWorkServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<IReadUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<ReadDbContext>>();
                var mediator = provider.GetRequiredService<IMediator>();
                var repositoryType = typeof(ReadRepository<>);

                return new UnitOfWork<ReadDbContext>(factory, mediator, repositoryType);
            });

            builder.Services.AddScoped<IWriteUnitOfWork>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<WriteDbContext>>();
                var mediator = provider.GetRequiredService<IMediator>();
                var repositoryType = typeof(WriteRepository<>);

                return new UnitOfWork<WriteDbContext>(factory, mediator, repositoryType);
            });
        }

        public static void AddMediatRServices(this IHostApplicationBuilder builder)
        {
            var assemblies = new Assembly[] { Assembly.Load(APPLICATION_ASSEMBLY) };

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assemblies);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
        }

        public static void AddGlobalExceptionHandlerServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        }

        public static void AddIdentityServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddRoles<Role>()
                .AddSignInManager();
        }

        public static void AddAuthenticationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var issuer = builder.Configuration["Jwt:Issuer"] ?? throw new NotFoundException("Jwt issuer not found");
                var audience = builder.Configuration["Jwt:Audience"] ?? throw new NotFoundException("Jwt audience not found");
                var key = builder.Configuration["Jwt:Key"] ?? throw new NotFoundException("Jwt key not found");

                options.TokenValidationParameters = Helpers.GetTokenValidationParameters(issuer, audience, key);
            });
        }

        public static void AddAuthorizationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(Schemes.UserScheme, builder =>
                {
                    builder.RequireAuthenticatedUser().RequireClaim(ClaimTypes.NameIdentifier);
                })
                .AddPolicy(ApiKeyRequirement.Scheme, policy =>
                {
                    var secret = builder.Configuration[ApiKeyRequirement.Scheme] ??
                        throw new NotFoundException("Api key not found");

                    policy.Requirements.Add(new ApiKeyRequirement(secret));
                });
        }

        public static void AddSwaggerServices(this IHostApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen(options =>
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

                builder.Services.AddSwaggerGenNewtonsoftSupport();
            }


        }

        public static void AddUserServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGuidGeneratorService, GuidGeneratorService>();
            builder.Services.AddScoped<IUserService, UserService>();
        }

        public static void AddRoutingServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddRouting(x => x.LowercaseUrls = true);
        }

        public static void AddAutoMapperServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(Assembly.Load(APPLICATION_ASSEMBLY));
        }
    }
}
