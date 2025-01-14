using PlayerHub.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAPIServices();
builder.AddApplicationServices();
builder.AddDataServices();
builder.AddIdentityAuthGuardServices();
builder.AddAPISecurityServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerConfiguration();

    await app.ApplyPenndingMigrationAsync();
}

await app.ApplySeedAsync();

app.UseExceptionHandler();
app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
