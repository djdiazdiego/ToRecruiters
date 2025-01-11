using Core.Data.Seeds;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using PlayerHub.API.Extensions;
using PlayerHub.Data.Contexts;
using System.Reflection;

namespace PlayerHub.API.Extensions
{
    internal static class WebApplicationExtensions
    {
        private const string DATA_ASSEMBLY = "PlayerHub.Data";
        private const string IDENTITY_ASSEMBLY = "IdentityAuthGuard";

        public static void UseSwaggerConfiguration(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "PlayerHub.API v1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.DefaultModelsExpandDepth(-1);
            });
        }

        public static void UseCors(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(p =>
                {
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                    p.AllowCredentials();
                    p.WithOrigins("http://localhost:4200");
                });
            }
        }

        public static async Task ApplyPenndingMigrationAsync(this WebApplication app)
        {
            using var source = new CancellationTokenSource();
            await ApplyPenndingMigrationAsync<AppDbContext>(app, source.Token);
            await ApplyPenndingMigrationAsync<WriteDbContext>(app, source.Token);
        }

        public static async Task ApplySeedAsync(this WebApplication app)
        {
            Assembly[] assemblies = [Assembly.Load(IDENTITY_ASSEMBLY), Assembly.Load(DATA_ASSEMBLY)];

            var types = typeof(ISeed).GetConcreteTypes(assemblies: assemblies);

            using var source = new CancellationTokenSource();

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is ISeed seed)
                {
                    await seed.SeedAsync(app.Services, source.Token);
                }
            }
        }

        private static async Task ApplyPenndingMigrationAsync<TContext>(
            WebApplication app,
            CancellationToken cancellationToken = default) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();

            if (scope.ServiceProvider.GetRequiredService<IDbContextFactory<TContext>>() is IDbContextFactory<TContext> factory)
            {
                using var context = factory.CreateDbContext();

                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory" &&
                    context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync(cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to get database context instance");
            }
        }
    }
}
