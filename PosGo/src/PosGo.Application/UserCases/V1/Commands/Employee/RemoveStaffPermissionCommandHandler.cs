using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Constants;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Employee;

public sealed class RemoveStaffPermissionCommandHandler : ICommandHandler<Command.RemoveStaffPermissionCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RemoveStaffPermissionCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        IRepositoryBase<Domain.Entities.RestaurantUser, int> restaurantUserRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(
        Command.RemoveStaffPermissionCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy restaurantId từ token
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error(
                "NO_RESTAURANT",
                "Bạn cần chọn cửa hàng trước."));
        }

        // 2. Lấy Staff
        var staff = await _userManager.FindByIdAsync(request.StaffId.ToString());
        if (staff is null)
        {
            return Result.Failure(new Error("NOT_FOUND", "Không tìm thấy nhân viên."));
        }

        // 3. Kiểm tra Staff thuộc Restaurant của Owner
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

        // 4. Kiểm tra PermissionKey hợp lệ
        if (!PermissionConstants.PermissionKeys.Contains(request.PermissionKey))
        {
            return Result.Failure(new Error(
                "INVALID_PERMISSION_KEY",
                $"PermissionKey không hợp lệ: {request.PermissionKey}"));
        }

        // 5. Lấy UserClaims của Staff
        var existingClaims = await _userManager.GetClaimsAsync(staff);
        var claimToRemove = existingClaims
            .FirstOrDefault(c => c.Type == request.PermissionKey);

        if (claimToRemove is null)
        {
            return Result.Failure(new Error(
                "NOT_FOUND",
                $"Nhân viên không có quyền {request.PermissionKey}."));
        }

        // 6. Xóa claim
        await _userManager.RemoveClaimAsync(staff, claimToRemove);

        return Result.Success();
    }
}
