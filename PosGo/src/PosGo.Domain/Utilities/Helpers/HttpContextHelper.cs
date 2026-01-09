using Microsoft.AspNetCore.Http;
using PosGo.Contract.Common.Constants;
using System;
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

    public static string GetScope(this HttpContext httpContext)
    {
        return httpContext?.User?.FindFirst(SystemConstants.ClaimTypes.SCOPE)?.Value 
            ?? SystemConstants.Scope.RESTAURANT;
    }

    /// <summary>
    /// Lấy RestaurantId từ JWT token claim "restaurant_id"
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <returns>RestaurantId nếu có, null nếu không có (Admin SYSTEM scope hoặc chưa switch restaurant)</returns>
    public static Guid? GetRestaurantId(this HttpContext httpContext)
    {
        var restaurantIdStr = httpContext?.User?.FindFirst(SystemConstants.ClaimTypes.RESTAURANT_ID)?.Value;
        
        if (Guid.TryParse(restaurantIdStr, out var restaurantId))
            return restaurantId;

        return null;
    }

    /// <summary>
    /// Key By Pass
    /// </summary>
    public static class TenantFilterBypass
    {
        public const string Key = "__BYPASS_TENANT_FILTER__";
    }

    /// <summary>
    /// Kiểm tra user có phải SYSTEM scope không (Admin)
    /// </summary>
    public static bool IsSystemScope(this HttpContext httpContext)
    {
        var scope = httpContext.GetScope();
        return scope.Equals(SystemConstants.Scope.SYSTEM, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Kiểm tra user có phải RESTAURANT scope không (Owner/Staff)
    /// </summary>
    public static bool IsRestaurantScope(this HttpContext httpContext)
    {
        var scope = httpContext.GetScope();
        return scope.Equals(SystemConstants.Scope.RESTAURANT, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Đảm bảo HttpContext có RestaurantId (throw exception nếu không có)
    /// Dùng trong các handler yêu cầu restaurant context
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <exception cref="InvalidOperationException">Nếu không có RestaurantId</exception>
    public static void EnsureRestaurantContext(this HttpContext httpContext)
    {
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            throw new InvalidOperationException(
                "Operation requires restaurant context. Please switch to a restaurant first.");
        }
    }

    /// <summary>
    /// Kiểm tra HttpContext có RestaurantId không (không throw exception)
    /// </summary>
    public static bool HasRestaurantContext(this HttpContext httpContext)
    {
        return httpContext.GetRestaurantId().HasValue;
    }
}
