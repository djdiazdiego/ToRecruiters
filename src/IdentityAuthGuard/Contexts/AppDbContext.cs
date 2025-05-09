using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAuthGuard.Contexts
{
    /// <summary>
    /// Represents the application's database context, inheriting from <see cref="IdentityDbContext{TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken}"/> 
    /// to provide identity-related functionality.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
    /// </remarks>
    /// <param name="options">The options to configure the database context.</param>
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) :
        IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
    {

        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="builder">The <see cref="ModelBuilder"/> used to configure the model.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set the default schema for the database.
            builder.HasDefaultSchema(DatabaseConstants.DEFAULT_SCHEMA);

            // Map identity-related entities to their respective database tables.
            builder.Entity<User>().ToTable(nameof(User));
            builder.Entity<Role>().ToTable(nameof(Role));
            builder.Entity<UserClaim>().ToTable(nameof(UserClaim));
            builder.Entity<UserRole>().ToTable(nameof(UserRole));
            builder.Entity<UserLogin>().ToTable(nameof(UserLogin));
            builder.Entity<RoleClaim>().ToTable(nameof(RoleClaim));
            builder.Entity<UserToken>().ToTable(nameof(UserToken));
        }
    }
}
