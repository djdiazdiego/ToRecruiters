using Core.Infrastructure.Common;
using Core.Infrastructure.Extensions;
using Core.Web.Extensions;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Data.Contexts;
using IdentityAuthGuard.Extensions;
using PlayerHub.API.Extensions;
using PlayerHub.Application.Extensions;
using PlayerHub.Infrastructure;
using PlayerHub.Infrastructure.Contexts;
using PlayerHub.Infrastructure.Extensions;
using Security;
using System.Reflection;

// Create a WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services.AddHealthChecksServices(builder.Configuration, Constants.CONNECTION_STRING, DbTypes.SqlServer); // Add health check services
builder.Services.AddWebServices(); // Add web services for logging, routing, controllers, exception handling, HSTS, rate limiting, and OpenTelemetry.
builder.Services.AddAPIServices(builder.Environment); // Add API-related services including CORS and Swagger
builder.Services.AddInfrastructureServices(builder.Configuration); // Add infrastructure services including DbContext factories and unit of work services
builder.Services.AddApplicationServices(); // Add application services for business logic and domain events
builder.Services.AddIdentityAuthGuardServices(); // Add Identity and AuthGuard services
builder.Services.AddAuthenticationServices(builder.Configuration); // Add authentication services
builder.Services.AddAuthorizationServices(builder.Configuration); // Add authorization services

// Build the application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Configure middleware for development environment
    app.UseDeveloperExceptionPage(); // Enable detailed error pages
    app.UseSwaggerConfiguration(); // Enable Swagger for API documentation

    // Apply pending migrations for development databases
    await app.Services.ApplyPenndingMigrationAsync([typeof(AppDbContext), typeof(WriteDbContext)]);
}
else
{
    // Enable HTTP Strict Transport Security
    app.UseHsts();

    // Redirect HTTP requests to HTTPS
    app.UseHttpsRedirection();
}

// Configure middleware for exception handling
app.UseExceptionHandler();
// Enable routing
app.UseRouting();
// Enable Cross-Origin Resource Sharing
app.UseCors(PlayerHub.API.Extensions.ServiceCollectionExtensions.PLAYER_HUB_CORS_POLICY);
// Enable rate limiting middleware
app.UseRateLimiter();

// Configure authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Apply seed data to the database
await app.Services.ApplySeedAsync([
    Assembly.Load(Constants.MIGRATIONS_ASSEMBLY), // Load migrations assembly for IdentityAuthGuard
    Assembly.Load(DatabaseConstants.MIGRATIONS_ASSEMBLY) // Load migrations assembly for PlayerHub.Data
]);

// Map API controllers to routes
app.MapControllers();

// Registers health check endpoints for monitoring application health
app.MapHealthChecks(DbTypes.SqlServer);

// Run the application
app.Run();
