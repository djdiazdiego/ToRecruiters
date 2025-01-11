using IdentityAuthGuard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PlayerHub.Data.Contexts
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) :
        IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("dbo.Identity");

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
