using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Team5Hackathon.API.Configuration;
using Team5Hackathon.API.Middleware;
using Team5Hackathon.Tests.Fakes;

namespace Team5Hackathon.Tests.Middleware;

public sealed class ApiKeyAuthenticationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsUnauthorized_WhenApiKeyIsMissing()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/users";
        context.Response.Body = new MemoryStream();
        var auditService = new RecordingAuditService();

        await middleware.InvokeAsync(context, auditService);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.False(nextCalled);
        Assert.Single(auditService.Entries);
        Assert.Equal("api.access", auditService.Entries[0].Action);
        Assert.Equal("failed", auditService.Entries[0].Outcome);
    }

    [Fact]
    public async Task InvokeAsync_CallsNext_ForPublicPath()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();
        context.Request.Path = "/health";
        context.Response.Body = new MemoryStream();
        var auditService = new RecordingAuditService();

        await middleware.InvokeAsync(context, auditService);

        Assert.True(nextCalled);
        Assert.Empty(auditService.Entries);
    }

    private static ApiKeyAuthenticationMiddleware CreateMiddleware(RequestDelegate next)
    {
        var options = Options.Create(new ApiSecurityOptions
        {
            HeaderName = "X-Api-Key",
            ApiKey = "secret-key"
        });

        return new ApiKeyAuthenticationMiddleware(next, options, NullLogger<ApiKeyAuthenticationMiddleware>.Instance);
    }
}
