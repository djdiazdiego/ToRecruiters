using Core.Data.Seeds;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAuthGuard.Seeds
{
    /// <summary>
    /// Represents a seed class for initializing default user roles and a default admin user.
    /// </summary>
    public sealed class UserRoleSeed : ISeed
    {
        /// <summary>
        /// Seeds default roles and a default admin user into the database.
        /// </summary>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            using var scope = provider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            foreach (var role in DefaultRoles.Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role(role) { });
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByEmailAsync(DefaultUsers.Email);

            if (user is null)
            {
                user = new User
                {
                    UserName = DefaultUsers.UserName,
                    Email = DefaultUsers.Email,
                    FirstName = DefaultUsers.FirstName,
                    LastName = DefaultUsers.LastName,
                    MiddleName = DefaultUsers.MiddleName,
                    SecondLastName = DefaultUsers.SecondLastName,
                    EmailConfirmed = DefaultUsers.EmailConfirmed
                };

                var result = await userManager.CreateAsync(user, DefaultUsers.Password);
                await userManager.AddToRoleAsync(user, DefaultRoles.Admin);
            }
        }
    }
}
