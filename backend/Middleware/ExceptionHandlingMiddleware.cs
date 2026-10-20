using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TurnosApi.Exceptions;

namespace TurnosApi.Middleware;

// MIDDLEWARE: se ejecuta en CADA request. Envuelve todo en un try/catch global,
// así los controllers no necesitan try/catch y los errores siempre salen con el mismo formato (ProblemDetails).
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);                                 // Sigue con el resto del pipeline
        }
        catch (AppException ex)                                   // Errores "esperados" (404, 400, 409...)
        {
            await Escribir(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)                                      // Errores inesperados -> 500
        {
            _logger.LogError(ex, "Error no controlado");
            await Escribir(context, 500, "Ocurrió un error interno en el servidor.");
        }
    }

    private static async Task Escribir(HttpContext context, int status, string detalle)
    {
        var problema = new ProblemDetails { Status = status, Title = "Error", Detail = detalle };
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problema,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
