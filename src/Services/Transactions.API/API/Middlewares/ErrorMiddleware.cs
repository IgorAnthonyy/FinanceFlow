using FinanceFlow.SharedKernel.Exceptions;

namespace Transactions.API.Middlewares;

/// <summary>
/// Middleware responsável por tratar exceções não capturadas na aplicação.
/// </summary>
public class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Construtor do middleware de tratamento de erro.
    /// </summary>
    /// <param name="next">O próximo middleware no pipeline.</param>
    /// <param name="env">O ambiente de execução da aplicação.</param>
    public ErrorMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    /// <summary>
    /// Invoca a execução do middleware.
    /// </summary>
    /// <param name="context">O contexto da requisição HTTP.</param>
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
    /// Trata a exceção e retorna a resposta formatada em JSON.
    /// </summary>
    /// <param name="context">O contexto da requisição HTTP.</param>
    /// <param name="exception">A exceção capturada.</param>
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
    /// Obtém o código de status HTTP correspondente ao tipo de exceção.
    /// </summary>
    /// <param name="exception">A exceção capturada.</param>
    /// <returns>O código de status HTTP.</returns>
    private static int GetStatusCode(Exception exception) => exception switch
    {
        UnauthorizedException => StatusCodes.Status401Unauthorized,
        ConflictException => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
