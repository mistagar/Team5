using System.Text.Json;
using Team5Hackathon.API.Constants;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.API.Factories;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", context.TraceIdentifier);

            await auditService.RecordAsync(
                AuditEntryFactory.Create(
                    context,
                    AuditActions.ApiError,
                    "failed",
                    "Unhandled exception occurred while processing request."),
                context.RequestAborted);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = ApiResponse<object>.FailResponse(
                "An unexpected error occurred.",
                new List<string> { "Please contact support with the correlation id from response headers." });

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
