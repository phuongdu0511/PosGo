using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Queries.Menu;

public class GetMenuByUserQueryHandler : IQueryHandler<GetMenuByUserQuery, List<GetMenuByUserResponse>>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IRepositoryBase<Domain.Entities.Function, int> _functionRepository;
    private readonly IPermissionService _permissionService;

    public GetMenuByUserQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.Function, int> functionRepository,
        IMapper mapper,
        IPermissionService permissionService)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _functionRepository = functionRepository;
        _permissionService = permissionService;
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
        var httpContext = _httpContextAccessor.HttpContext;
        var scope = httpContext.GetScope();
        var restaurantId = httpContext.GetRestaurantId();
        
        var permissionIntMap = await _permissionService.GetUserPermissionsAsync(user, scope, restaurantId);

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

}
