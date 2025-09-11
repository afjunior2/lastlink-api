using System.Net;
using System.Text.Json;
using LastLinkApi.Api.DTOs;

namespace LastLinkApi.Api.Middleware;

/// <summary>
/// Middleware global de tratamento de erros
/// </summary>
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // continua o pipeline normalmente
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Erro de validação: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "O valor solicitado deve ser maior que R$100,00.");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Erro de negócio: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.Conflict, "Operação inválida para esta solicitação.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponseDto { Message = message };
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}