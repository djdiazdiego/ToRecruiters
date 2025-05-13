namespace PlayerHub.API.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring the web application.
    /// </summary>
    internal static class MiddlewareExtensions
    {
        /// <summary>
        /// Configures Swagger and Swagger UI for the application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance to configure.</param>
        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Sets the endpoint for the Swagger JSON document.
                c.SwaggerEndpoint("swagger/v1/swagger.json", "PlayerHub.API v1");

                // Sets the Swagger UI route prefix to the root.
                c.RoutePrefix = string.Empty;

                // Configures the Swagger UI to collapse all sections by default.
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

                // Disables the display of schemas in the Swagger UI.
                c.DefaultModelsExpandDepth(-1);
            });
        }
    }
}
