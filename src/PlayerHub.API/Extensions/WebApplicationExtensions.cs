using PlayerHub.API.Extensions;

namespace PlayerHub.API.Extensions
{
    internal static class WebApplicationExtensions
    {
        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
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

        public static void UseCors(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
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
    }
}
