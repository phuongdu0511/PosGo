using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Menu;

public class GetMenuByUserQueryHandler : IQueryHandler<GetMenuByUserQuery, List<GetMenuByUserResponse>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IRepositoryBase<Domain.Entities.Function, int> _functionRepository;
    private readonly ApplicationDbContext _dbContext;
    public GetMenuByUserQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        RoleManager<Domain.Entities.Role> roleManager,
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.Function, int> functionRepository,
        IMapper mapper,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _functionRepository = functionRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<List<GetMenuByUserResponse>>> Handle(GetMenuByUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new CommonNotFoundException.CommonException(userId, "User");
        var menus = await GetMenusByUserAsync(user);
        var result = _mapper.Map<List<GetMenuByUserResponse>>(menus);
        return Result.Success(result);
    }

    private async Task<List<Domain.Entities.Function>> GetMenusByUserAsync(Domain.Entities.User user)
    {
        // 1. Lấy map PermissionKey -> int ActionValue user có
        var permissionIntMap = await GetPermissionIntByUserAsync(user);

        // 2. Lấy tất cả Menu đang Active (có thể thêm điều kiện Status == Active)
        var allMenus = await _functionRepository.FindAll(x => x.Status == Status.Active)
            .OrderBy(f => f.SortOrder)
            .ToListAsync();

        // 3. Lọc các Menu user có quyền
        var allowedSet = allMenus
            .Where(f =>
                !string.IsNullOrEmpty(f.Key) &&
                permissionIntMap.TryGetValue(f.Key, out var userActionValue) &&
                HasPermissionForMenu(userActionValue, f.ActionValue))
            .Select(x => x.Id)
            .ToHashSet();

        // 4) Dựng tree: nếu con được phép thì kéo cha lên (để menu không bị mồ côi)
        //    - Chỉ trả về các node thuộc nhánh có quyền
        var byId = allMenus.ToDictionary(x => x.Id, x => x);

        void IncludeAncestors(int id, HashSet<int> set)
        {
            if (!byId.TryGetValue(id, out var node)) return;

            // Nếu node đã được include thì thôi
            if (!set.Add(node.Id)) return;

            // kéo cha (ParrentId) lên
            if (node.ParrentId > 0)
            {
                IncludeAncestors(node.ParrentId, set);
            }
        }

        // clone set để add cha
        var finalSet = new HashSet<int>(allowedSet);
        foreach (var id in allowedSet)
        {
            IncludeAncestors(id, finalSet);
        }

        // 5) Trả ra danh sách menu theo thứ tự (vẫn flat), FE có thể build tree từ ParrentId
        var available = allMenus
            .Where(f => finalSet.Contains(f.Id))
            .OrderBy(f => f.SortOrder)
            .ToList();

        return available;
    }

    /// <summary>
    /// userActionValue: bitmask quyền user có cho PermissionKey đó (VD: 7 = 111 = View+Add+Update)
    /// MenuActionValue: bitmask yêu cầu để thấy menu (VD: 1 = View, hoặc 3 = View+Add)
    /// </summary>
    private bool HasPermissionForMenu(int userActionValue, int MenuActionValue)
    {
        // Phase 1: chỉ cần user có ÍT NHẤT 1 quyền trùng với yêu cầu:
        return (userActionValue & MenuActionValue) != 0;

        // Phase 2: bắt buộc user phải có ĐỦ toàn bộ bit mà Menu yêu cầu:
        //return (userActionValue & MenuActionValue) == MenuActionValue;
    }

    private async Task<Dictionary<string, int>> GetPermissionIntByUserAsync(Domain.Entities.User user)
    {
        var resultInt = new Dictionary<string, int>();

        // 1) Lấy claims
        var roleClaims = await GetClaimsByUserRoleAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any())
            roleClaims.AddRange(userClaims);

        // 2) Lấy restaurant_id hiện tại (đang active) từ HttpContext token
        //    (Nếu SYSTEM scope thì bạn bỏ qua plan, trả full quyền theo claims)
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantIdStr = httpContext?.User?.FindFirst("restaurant_id")?.Value;

        // SYSTEM scope: không cần restaurant => trả toàn bộ claims hợp nhất
        var scope = httpContext?.User?.FindFirst("scope")?.Value; // nếu bạn có claim scope
        if (string.Equals(scope, SystemConstants.Scope.SYSTEM, StringComparison.OrdinalIgnoreCase))
        {
            return BuildClaimIntMap(roleClaims);
        }

        // Nếu không có restaurant_id thì chỉ return các quyền "global" (vd SwitchRestaurant) nếu bạn muốn
        if (!Guid.TryParse(restaurantIdStr, out var restaurantId))
        {
            // ví dụ: chỉ cho phép SwitchRestaurant nếu claim có
            var map = BuildClaimIntMap(roleClaims);
            var results = new Dictionary<string, int>();

            if (map.TryGetValue(PermissionConstants.SwitchRestaurant, out var v))
                results.Add(PermissionConstants.SwitchRestaurant, v);

            return results;
        }

        // 3) Lấy danh sách function key thuộc plan của restaurant đang active
        //    key này phải trùng với claim.Type và Function.Key
        var planFunctionKeys = await (
            from rp in _dbContext.RestaurantPlans
            join pf in _dbContext.PlanFunctions on rp.PlanId equals pf.PlanId
            join f in _dbContext.Functions on pf.FunctionId equals f.Id
            where rp.RestaurantId == restaurantId
                  && rp.IsActive
                  && f.Status == Status.Active
            select f.Key
        ).Distinct().ToListAsync();

        var planKeySet = planFunctionKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // 4) Build map từ claims rồi intersect theo plan
        var fullClaimMap = BuildClaimIntMap(roleClaims);

        foreach (var kv in fullClaimMap)
        {
            if (planKeySet.Contains(kv.Key))
            {
                resultInt[kv.Key] = kv.Value;
            }
        }

        return resultInt;
    }

    private Dictionary<string, int> BuildClaimIntMap(List<Claim> roleClaims)
    {
        var resultInt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var roleClaimNames = roleClaims.Select(x => x.Type).Distinct();

        foreach (var name in roleClaimNames)
        {
            if (!resultInt.ContainsKey(name))
            {
                var claims = roleClaims.Where(p => p.Type == name);

                if (!claims.Any(p => p.Value == PermissionConstants.Deny.ToString()))
                {
                    var value = 0;
                    foreach (var claim in claims)
                    {
                        if (int.TryParse(claim.Value, out int claimValue))
                            value |= claimValue;
                    }
                    resultInt.Add(name, value);
                }
            }
        }

        return resultInt;
    }

    private async Task<List<Claim>> GetClaimsByUserRoleAsync(Domain.Entities.User user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = await _roleManager.Roles.Where(p => roleNames.Contains(p.Name)).ToListAsync();
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            var resultClaims = await _roleManager.GetClaimsAsync(role);
            if (resultClaims.Any())
            {
                roleClaims.AddRange(resultClaims);
            }
        }
        return roleClaims;
    }
}
