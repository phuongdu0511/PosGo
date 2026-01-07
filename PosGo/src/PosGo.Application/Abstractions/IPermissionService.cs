using PosGo.Domain.Entities;

namespace PosGo.Application.Abstractions;

/// <summary>
/// Service để tính toán và lấy quyền của user dựa trên RoleClaim, UserClaim, Scope và Plan
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Lấy dictionary PermissionKey -> ActionValue (bitmask) của user
    /// </summary>
    /// <param name="user">User cần lấy quyền</param>
    /// <param name="scope">SYSTEM hoặc RESTAURANT</param>
    /// <param name="restaurantId">RestaurantId nếu có (null nếu Admin hoặc Owner chưa switch)</param>
    /// <returns>Dictionary với key là PermissionKey, value là ActionValue (bitmask)</returns>
    Task<Dictionary<string, int>> GetUserPermissionsAsync(User user, string scope, Guid? restaurantId);
}
