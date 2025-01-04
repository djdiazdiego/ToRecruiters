using PlayerHub.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggingServices();

builder.AddCorsServices();
builder.AddControllerServices();

builder.AddHttpContextAccessorServices();

builder.AddAutoMapperServices();
builder.AddMediatRServices();

builder.AddDbContextFactoryServices();
builder.AddUnitOfWorkServices();

builder.AddIdentityServices();
builder.AddUserServices();

builder.AddAuthenticationServices();
builder.AddAuthorizationServices();

builder.AddRoutingServices();

builder.AddGlobalExceptionHandlerServices();

builder.AddSwaggerServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerConfiguration();

    await app.ApplyPenndingMigrationAsync();
}

await app.ApplySeedAsync();

app.UseExceptionHandler();

//app.UseCors();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
