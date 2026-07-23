using Identity.API.Middlewares;

namespace Identity.API.Extensions;

/// <summary>
/// Classe responsável por configurar o pipeline do application.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline do application.
    /// </summary>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Identity.API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseCors("AllowFrontend");
        app.UseMiddleware<ErrorMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}