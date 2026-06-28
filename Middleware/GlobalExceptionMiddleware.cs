using System.Net;
using System.Text.Json;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Logging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = GetErrorResponse(exception);

        if (statusCode >= (int)HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "{EventName}: {Message}", LogEventNames.UnhandledException, exception.Message);
        }
        else
        {
            logger.LogWarning(exception, "Handled API exception: {Message}", exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var errors = environment.IsDevelopment()
            ? new[] { exception.Message }
            : Array.Empty<string>();

        var response = ApiResponse<object>.Fail(message, errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (int StatusCode, string Message) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Authentication failed."),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            BadHttpRequestException => ((int)HttpStatusCode.BadRequest, exception.Message),
            DbUpdateException => ((int)HttpStatusCode.Conflict, "A database update conflict occurred."),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}
