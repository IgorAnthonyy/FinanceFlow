using Transactions.API.Middlewares;

namespace Transactions.API.Extensions;

/// <summary>
/// Classe responsável por configurar o pipeline da aplicação Transactions.API.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline de requisições HTTP da aplicação.
    /// </summary>
    /// <param name="app">A aplicação Web.</param>
    /// <returns>A aplicação Web configurada.</returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Transactions.API v1");
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
