using Team5Hackathon.API.Middleware;

namespace Team5Hackathon.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiSecurityPipeline(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

        return app;
    }
}
