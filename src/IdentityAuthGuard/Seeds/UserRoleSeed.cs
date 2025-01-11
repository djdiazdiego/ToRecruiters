using Core.Data.Seeds;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAuthGuard.Seeds
{
    public sealed class UserRoleSeed : ISeed
    {
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
