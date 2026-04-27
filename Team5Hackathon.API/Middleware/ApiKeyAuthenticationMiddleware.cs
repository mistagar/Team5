using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Team5Hackathon.API.Configuration;
using Team5Hackathon.API.Constants;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.API.Factories;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Middleware;

public sealed class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiSecurityOptions _apiSecurityOptions;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IOptions<ApiSecurityOptions> apiSecurityOptions,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _apiSecurityOptions = apiSecurityOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        if (IsAnonymousEndpoint(context))
        {
            await _next(context);
            return;
        }

        if (IsPublicPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(_apiSecurityOptions.ApiKey))
        {
            _logger.LogError("API security is misconfigured. {Section} is missing.", ApiSecurityOptions.SectionName);
            await WriteUnauthorizedAsync(context, "API security configuration is invalid.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(_apiSecurityOptions.HeaderName, out var providedApiKey)
            || !TimeSafeEquals(providedApiKey.ToString(), _apiSecurityOptions.ApiKey))
        {
            await auditService.RecordAsync(
                AuditEntryFactory.Create(
                    context,
                    AuditActions.ApiAccess,
                    "failed",
                    "Unauthorized API access attempt."),
                context.RequestAborted);

            await WriteUnauthorizedAsync(context, "Unauthorized request.");
            return;
        }

        await _next(context);
    }

    private static bool IsAnonymousEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null;
    }

    private static bool TimeSafeEquals(string candidate, string expected)
    {
        var candidateBytes = System.Text.Encoding.UTF8.GetBytes(candidate);
        var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
        return candidateBytes.Length == expectedBytes.Length
               && CryptographicOperations.FixedTimeEquals(candidateBytes, expectedBytes);
    }

    private static bool IsPublicPath(PathString path)
    {
        return path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
               || path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var payload = ApiResponse<object>.FailResponse(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
