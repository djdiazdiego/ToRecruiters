using Core.Data.Extensions;
using Core.Security;
using IdentityAuthGuard.Constants;
using IdentityAuthGuard.Contexts;
using IdentityAuthGuard.Extensions;
using PlayerHub.API.Extensions;
using PlayerHub.Application.Extensions;
using PlayerHub.Data;
using PlayerHub.Data.Contexts;
using PlayerHub.Data.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAPIServices(builder.Environment);
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddIdentityAuthGuardServices();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorizationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerConfiguration();

    await app.Services.ApplyPenndingMigrationAsync([typeof(AppDbContext), typeof(WriteDbContext)]);
}

await app.Services.ApplySeedAsync([
    Assembly.Load(Constants.MIGRATIONS_ASSEMBLY),
    Assembly.Load(DatabaseConstants.MIGRATIONS_ASSEMBLY)]);

app.UseExceptionHandler();
app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
