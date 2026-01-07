using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Employee;

public sealed class UpdateStaffPermissionsCommandHandler : ICommandHandler<Command.UpdateStaffPermissionsCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public UpdateStaffPermissionsCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        IRepositoryBase<Domain.Entities.RestaurantUser, int> restaurantUserRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<Result> Handle(
        Command.UpdateStaffPermissionsCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin Owner đang đăng nhập
        var httpContext = _httpContextAccessor.HttpContext;
        var ownerId = httpContext.GetCurrentUserId();
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error(
                "NO_RESTAURANT",
                "Bạn cần chọn cửa hàng trước khi cấp quyền cho nhân viên."));
        }

        // 2. Lấy Staff
        var staff = await _userManager.FindByIdAsync(request.StaffId.ToString());
        if (staff is null)
        {
            return Result.Failure(new Error("NOT_FOUND", "Không tìm thấy nhân viên."));
        }

        // 3. Kiểm tra Staff có thuộc Restaurant của Owner không
        var staffRestaurantUser = await _restaurantUserRepository.FindSingleAsync(
            x => x.UserId == request.StaffId
              && x.RestaurantId == restaurantId.Value
              && x.Status == ERestaurantUserStatus.Active,
            cancellationToken);

        if (staffRestaurantUser is null)
        {
            return Result.Failure(new Error(
                "FORBIDDEN",
                "Nhân viên này không thuộc cửa hàng của bạn."));
        }

        // 4. Kiểm tra Staff thực sự là Staff
        var staffRoleNames = await _userManager.GetRolesAsync(staff);
        var isStaffRole = staffRoleNames.Any(r =>
            r.Equals(SystemConstants.RoleName.STAFF, StringComparison.OrdinalIgnoreCase) ||
            r.Equals(SystemConstants.RoleName.MANAGER, StringComparison.OrdinalIgnoreCase));

        if (!isStaffRole)
        {
            return Result.Failure(new Error(
                "INVALID_ROLE",
                "Chỉ có thể cấp quyền cho Staff hoặc Manager."));
        }

        // 5. Lấy Plan Functions của Restaurant để validate permissions
        var planFunctionKeys = await GetPlanFunctionKeys(restaurantId.Value);

        // 6. Lấy Functions để validate ActionValue
        var functions = await _context.Functions
            .Where(f => f.Status == Status.Active)
            .ToDictionaryAsync(f => f.Key, f => f.ActionValue, cancellationToken);

        // 7. Validate permissions
        var invalidPermissions = new List<string>();
        foreach (var permission in request.Permissions)
        {
            // 7.1. Kiểm tra PermissionKey có trong Plan không
            if (!planFunctionKeys.Contains(permission.PermissionKey))
            {
                invalidPermissions.Add($"{permission.PermissionKey}: Không có trong gói của cửa hàng");
                continue;
            }

            // 7.2. Kiểm tra Function tồn tại
            if (!functions.TryGetValue(permission.PermissionKey, out var maxActionValue))
            {
                invalidPermissions.Add($"{permission.PermissionKey}: Không tìm thấy Function");
                continue;
            }

            // 7.3. Kiểm tra ActionValue không vượt quá Function.ActionValue
            if (permission.ActionValue > maxActionValue)
            {
                invalidPermissions.Add($"{permission.PermissionKey}: ActionValue ({permission.ActionValue}) vượt quá giới hạn ({maxActionValue})");
                continue;
            }

            // 7.4. Kiểm tra ActionValue hợp lệ (phải là tổ hợp của View/Add/Update/Delete)
            if (permission.ActionValue <= 0 || permission.ActionValue > 15)
            {
                invalidPermissions.Add($"{permission.PermissionKey}: ActionValue không hợp lệ");
            }
        }

        if (invalidPermissions.Any())
        {
            return Result.Failure(new Error(
                "INVALID_PERMISSIONS",
                $"Các quyền không hợp lệ:\n{string.Join("\n", invalidPermissions)}"));
        }

        // 8. Xóa tất cả UserClaims cũ của Staff (chỉ xóa permission claims)
        var existingClaims = await _userManager.GetClaimsAsync(staff);
        var permissionClaimsToRemove = existingClaims
            .Where(c => PermissionConstants.PermissionKeys.Contains(c.Type))
            .ToList();

        foreach (var claim in permissionClaimsToRemove)
        {
            await _userManager.RemoveClaimAsync(staff, claim);
        }

        // 9. Thêm UserClaims mới
        foreach (var permission in request.Permissions)
        {
            if (permission.ActionValue > 0)
            {
                await _userManager.AddClaimAsync(staff,
                    new Claim(permission.PermissionKey, permission.ActionValue.ToString()));
            }
        }

        return Result.Success();
    }

    private async Task<HashSet<string>> GetPlanFunctionKeys(Guid restaurantId)
    {
        var keys = await (
            from rp in _context.RestaurantPlans
            join pf in _context.PlanFunctions on rp.PlanId equals pf.PlanId
            join f in _context.Functions on pf.FunctionId equals f.Id
            where rp.RestaurantId == restaurantId
                  && rp.IsActive
                  && f.Status == Status.Active
            select f.Key
        ).Distinct().ToListAsync();

        return keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
