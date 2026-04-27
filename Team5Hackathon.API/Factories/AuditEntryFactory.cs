using System.Security.Claims;
using Team5Hackathon.Application.DTOs.Audit;

namespace Team5Hackathon.API.Factories;

public static class AuditEntryFactory
{
    public static AuditEntry Create(
        HttpContext httpContext,
        string action,
        string outcome,
        string? description = null,
        string? entityName = null,
        string? entityId = null)
    {
        return new AuditEntry
        {
            Action = action,
            Outcome = outcome,
            Description = description,
            EntityName = entityName,
            EntityId = entityId,
            CorrelationId = httpContext.TraceIdentifier,
            PerformedBy = ResolveUserName(httpContext.User),
            PerformedById = ResolveUserId(httpContext.User),
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext.Request.Headers.UserAgent.ToString()
        };
    }

    private static string? ResolveUserName(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return "anonymous";
        }

        return user.Identity.Name
               ?? user.FindFirstValue("preferred_username")
               ?? user.FindFirstValue(ClaimTypes.Email)
               ?? user.FindFirstValue(ClaimTypes.Name);
    }

    private static string? ResolveUserId(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user.FindFirstValue("sub")
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue("oid");
    }
}
