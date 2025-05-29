using Core.Data.Helpers;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Data.Contexts;
using IdentityAuthGuard.Models;
using IdentityAuthGuard.Services.GuidGeneratorServices;
using IdentityAuthGuard.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAuthGuard.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring services in the IdentityAuthGuard application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all IdentityAuthGuard-related services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public static void AddIdentityAuthGuardServices(this IServiceCollection services)
        {
            services.AddIdentityServices();
            services.AddUserServices();
        }

        /// <summary>
        /// Configures Identity services, including user and role management, password policies, and sign-in requirements.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public static void AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8; // Sets the minimum password length.
                options.User.RequireUniqueEmail = true; // Ensures that each user has a unique email address.
                options.SignIn.RequireConfirmedEmail = true; // Requires users to confirm their email before signing in.
            })
                .AddEntityFrameworkStores<AppDbContext>() // Configures Entity Framework as the store for Identity.
                .AddRoles<Role>() // Enables role management.
                .AddSignInManager(); // Adds the sign-in manager for handling user authentication.
        }

        /// <summary>
        /// Configures user-related services, such as GUID generation and user management.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public static void AddUserServices(this IServiceCollection services)
        {
            services.AddSingleton<IGuidGeneratorService, GuidGeneratorService>(); // Registers a singleton service for generating GUIDs.
            services.AddScoped<IUserService, UserService>(); // Registers a scoped service for managing users.
        }

        /// <summary>
        /// Configures the application database context factory services, including connection string validation.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The application configuration containing connection strings.</param>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is not found in the configuration.</exception>
        public static void AddIdentityAuthGuardDbContextFactoryServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connection = configuration[$"ConnectionStrings:{DatabaseConstants.CONNECTION_STRING}"];

            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("The connection string was not found in the configuration collection.");
            }

            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                DbContextHelpers.ConfigureDbContextOptions<AppDbContext>(
                    connection,
                    DatabaseConstants.MIGRATIONS_ASSEMBLY,
                    DatabaseConstants.DB_TYPE,
                    options);
            });

            services.AddSingleton<IDbContextFactory<AppDbContext>>(new AppDbContextFactory(configuration));
        }
    }
}
