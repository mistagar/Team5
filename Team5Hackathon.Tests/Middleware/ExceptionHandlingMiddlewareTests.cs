using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Team5Hackathon.API.Middleware;
using Team5Hackathon.Tests.Fakes;

namespace Team5Hackathon.Tests.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Returns500AndAudits_WhenUnhandledExceptionOccurs()
    {
        RequestDelegate next = _ => throw new InvalidOperationException("boom");
        var middleware = new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var auditService = new RecordingAuditService();

        await middleware.InvokeAsync(context, auditService);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Single(auditService.Entries);
        Assert.Equal("api.error", auditService.Entries[0].Action);
        Assert.Equal("failed", auditService.Entries[0].Outcome);
    }
}
