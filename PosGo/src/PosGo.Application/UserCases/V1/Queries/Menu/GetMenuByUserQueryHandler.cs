using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Queries.Menu;

public class GetMenuByUserQueryHandler : IQueryHandler<GetMenuByUserQuery, List<GetMenuByUserResponse>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IRepositoryBase<Domain.Entities.Function, int> _functionRepository;
    public GetMenuByUserQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        RoleManager<Domain.Entities.Role> roleManager,
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.Function, int> functionRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _functionRepository = functionRepository;
    }

    public async Task<Result<List<GetMenuByUserResponse>>> Handle(GetMenuByUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new CommonNotFoundException.CommonException(userId, "User");
        var Menus = await GetMenusByUserAsync(user);
        var result = _mapper.Map<List<GetMenuByUserResponse>>(Menus);
        return Result.Success(result);
    }

    private async Task<List<Domain.Entities.Function>> GetMenusByUserAsync(Domain.Entities.User user)
    {
        // 1. Lấy map PermissionKey -> int ActionValue user có
        var permissionIntMap = await GetPermissionIntByUserAsync(user);

        // 2. Lấy tất cả Menu đang Active (có thể thêm điều kiện Status == Active)
        var allMenus = await _functionRepository.FindAll(x => x.Status == Status.Active)
            .OrderByDescending(f => f.SortOrder)
            .ToListAsync();

        // 3. Lọc các Menu user có quyền
        var available = allMenus
            .Where(f =>
                !string.IsNullOrEmpty(f.Key) &&
                permissionIntMap.TryGetValue(f.Key, out var userActionValue) &&
                HasPermissionForMenu(userActionValue, f.ActionValue))
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

        var roleClaims = await GetClaimsByUserRoleAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any())
            roleClaims.AddRange(userClaims);

        var roleClaimNames = roleClaims.Select(x => x.Type).Distinct();

        foreach (var name in roleClaimNames)
        {
            if (!resultInt.ContainsKey(name))
            {
                var claims = roleClaims.Where(p => p.Type == name);

                // nếu có claim Deny thì bỏ luôn quyền này
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
