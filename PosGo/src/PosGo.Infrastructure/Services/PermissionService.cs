using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Application.Abstractions;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Constants;
using PosGo.Persistence;

namespace PosGo.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public PermissionService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, int>> GetUserPermissionsAsync(User user, string scope, Guid? restaurantId)
    {
        // 1. Lấy tất cả claims (RoleClaim + UserClaim)
        var allClaims = await GetAllUserClaimsAsync(user);

        // 2. Xử lý theo Scope
        if (scope == SystemConstants.Scope.SYSTEM)
        {
            return BuildPermissionMap(allClaims);
        }

        // 3. RESTAURANT scope: xử lý theo restaurantId
        if (!restaurantId.HasValue)
        {
            // Chưa switch restaurant → chỉ trả quyền global
            return GetGlobalPermissions(allClaims);
        }

        // 4. Có restaurantId → lọc theo Plan
        return await GetRestaurantPermissionsAsync(allClaims, restaurantId.Value);
    }

    /// <summary>
    /// Lấy tất cả claims của user (RoleClaim + UserClaim)
    /// </summary>
    private async Task<List<Claim>> GetAllUserClaimsAsync(User user)
    {
        var roleClaims = await GetRoleClaimsAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any())
        {
            roleClaims.AddRange(userClaims);
        }

        return roleClaims;
    }

    /// <summary>
    /// Lấy RoleClaims của user (từ các role user đang có)
    /// </summary>
    private async Task<List<Claim>> GetRoleClaimsAsync(User user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = await _roleManager.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();

        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            if (claims.Any())
            {
                roleClaims.AddRange(claims);
            }
        }

        return roleClaims;
    }

    /// <summary>
    /// Build permission map từ claims (không filter)
    /// </summary>
    private Dictionary<string, int> BuildPermissionMap(List<Claim> claims)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var permissionKeys = claims.Select(c => c.Type).Distinct();

        foreach (var key in permissionKeys)
        {
            if (!result.ContainsKey(key))
            {
                var keyClaims = claims.Where(c => c.Type == key).ToList();

                // Nếu có claim Deny thì bỏ qua
                if (keyClaims.Any(c => c.Value == PermissionConstants.Deny.ToString()))
                {
                    continue;
                }

                // Merge tất cả action values (bitwise OR)
                var actionValue = 0;
                foreach (var claim in keyClaims)
                {
                    if (int.TryParse(claim.Value, out var claimValue))
                    {
                        actionValue |= claimValue;
                    }
                }

                if (actionValue > 0)
                {
                    result[key] = actionValue;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy quyền global (chỉ SwitchRestaurant khi chưa có restaurantId)
    /// </summary>
    private Dictionary<string, int> GetGlobalPermissions(List<Claim> claims)
    {
        var fullMap = BuildPermissionMap(claims);
        var globalKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            PermissionConstants.SwitchRestaurant
        };

        return fullMap
            .Where(kv => globalKeys.Contains(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    /// <summary>
    /// Lấy quyền của restaurant (lọc theo Plan)
    /// </summary>
    private async Task<Dictionary<string, int>> GetRestaurantPermissionsAsync(
        List<Claim> claims,
        Guid restaurantId)
    {
        // 1. Lấy danh sách Function Keys mà Plan của restaurant cho phép
        var planFunctionKeys = await GetPlanFunctionKeysAsync(restaurantId);

        // 2. Build permission map từ claims
        var fullMap = BuildPermissionMap(claims);

        // 3. Chỉ trả về permissions thuộc Plan
        var result = new Dictionary<string, int>();
        foreach (var kv in fullMap)
        {
            if (planFunctionKeys.Contains(kv.Key))
            {
                result[kv.Key] = kv.Value;
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách Function Keys mà Plan của restaurant đang active cho phép
    /// </summary>
    private async Task<HashSet<string>> GetPlanFunctionKeysAsync(Guid restaurantId)
    {
        var keys = await (
            from rp in _dbContext.RestaurantPlans
            join pf in _dbContext.PlanFunctions on rp.PlanId equals pf.PlanId
            join f in _dbContext.Functions on pf.FunctionId equals f.Id
            where rp.RestaurantId == restaurantId
                  && rp.IsActive
                  && f.Status == Status.Active
            select f.Key
        ).Distinct().ToListAsync();

        return keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
