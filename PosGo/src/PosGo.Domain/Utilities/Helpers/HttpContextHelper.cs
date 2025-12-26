using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PosGo.Domain.Utilities.Helpers;

public static class HttpContextHelper
{
    public static Guid GetCurrentUserId(this HttpContext httpContext)
    {
        var currentUserId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(currentUserId, out Guid userId);
        return userId;
    }

    public static string GetCurrentUserName(this HttpContext httpContext)
    {
        return httpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    }
}
