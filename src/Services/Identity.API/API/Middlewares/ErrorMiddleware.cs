using FinanceFlow.SharedKernel.Exceptions;

namespace Identity.API.Middlewares;

/// <summary>
/// Middleware responsável por tratar as exceções que ocorrem na aplicação.
/// </summary>
public class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Construtor do middleware.
    /// </summary>
    /// <param name="next">O próximo middleware.</param>
    /// <param name="env">O ambiente da aplicação.</param>
    public ErrorMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    /// <summary>
    /// Invoca o middleware.
    /// </summary>
    /// <param name="context">O contexto da requisição.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Trata a exceção.
    /// </summary>
    /// <param name="context">O contexto da requisição.</param>
    /// <param name="exception">A exceção.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = GetStatusCode(exception);

        var response = new
        {
            message = exception.Message,
            stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>
    /// Obtém o código de status HTTP para a exceção.
    /// </summary>
    /// <param name="exception">A exceção.</param>
    /// <returns>O código de status HTTP.</returns>
    private static int GetStatusCode(Exception exception) => exception switch
    {
        UnauthorizedException => StatusCodes.Status401Unauthorized,
        ConflictException => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}