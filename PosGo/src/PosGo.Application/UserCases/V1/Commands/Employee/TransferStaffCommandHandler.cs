using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Employee;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Employee;

public sealed class TransferStaffCommandHandler : ICommandHandler<Command.TransferStaffCommand>
{
    private readonly IRepositoryBase<RestaurantUser, int> _restaurantUserRepository;
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public TransferStaffCommandHandler(
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository,
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context)
    {
        _restaurantUserRepository = restaurantUserRepository;
        _restaurantRepository = restaurantRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<Result> Handle(
        Command.TransferStaffCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy OwnerId từ token
        var httpContext = _httpContextAccessor.HttpContext;
        var ownerId = httpContext.GetCurrentUserId();

        // 2. Kiểm tra FromRestaurant và ToRestaurant thuộc Owner
        var fromRestaurant = await _restaurantRepository.FindByIdAsync(request.FromRestaurantId);
        if (fromRestaurant is null || fromRestaurant.OwnerUserId != ownerId)
        {
            return Result.Failure(new Error(
                "FORBIDDEN",
                "Bạn không có quyền sở hữu cửa hàng nguồn."));
        }

        var toRestaurant = await _restaurantRepository.FindByIdAsync(request.ToRestaurantId);
        if (toRestaurant is null || toRestaurant.OwnerUserId != ownerId)
        {
            return Result.Failure(new Error(
                "FORBIDDEN",
                "Bạn không có quyền sở hữu cửa hàng đích."));
        }

        // 3. Kiểm tra Staff có thuộc FromRestaurant không
        var staffRestaurantUser = await _restaurantUserRepository.FindSingleAsync(
            x => x.UserId == request.StaffId
              && x.RestaurantId == request.FromRestaurantId
              && x.Status == ERestaurantUserStatus.Active,
            cancellationToken);

        if (staffRestaurantUser is null)
        {
            return Result.Failure(new Error(
                "NOT_FOUND",
                "Nhân viên không thuộc cửa hàng nguồn."));
        }

        // 4. Kiểm tra Staff thực sự là Staff (không phải Owner)
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == staffRestaurantUser.RoleId, cancellationToken);

        if (role is null || role.RoleCode == SystemConstants.RoleCode.OWNER)
        {
            return Result.Failure(new Error(
                "INVALID_ROLE",
                "Chỉ có thể chuyển Staff hoặc Manager."));
        }

        // 5. Kiểm tra Staff chưa thuộc ToRestaurant (nếu đã có thì update, không tạo mới)
        var existingInToRestaurant = await _restaurantUserRepository.FindSingleAsync(
            x => x.UserId == request.StaffId
              && x.RestaurantId == request.ToRestaurantId,
            cancellationToken);

        if (existingInToRestaurant is not null)
        {
            // Đã tồn tại → chỉ cần activate và update role nếu cần
            if (existingInToRestaurant.Status != ERestaurantUserStatus.Active)
            {
                existingInToRestaurant.Activate();
            }
            if (existingInToRestaurant.RoleId != staffRestaurantUser.RoleId)
            {
                existingInToRestaurant.ChangeRole(staffRestaurantUser.RoleId);
            }
        }
        else
        {
            // Chưa tồn tại → tạo mới
            var newRestaurantUser = RestaurantUser.Create(
                restaurantId: request.ToRestaurantId,
                userId: request.StaffId,
                roleId: staffRestaurantUser.RoleId);

            _restaurantUserRepository.Add(newRestaurantUser);
        }

        // 6. Deactivate hoặc xóa mapping ở FromRestaurant
        // Option 1: Deactivate (giữ lại lịch sử)
        staffRestaurantUser.Deactivate();

        // Option 2: Xóa hoàn toàn (nếu muốn)
        // _restaurantUserRepository.Remove(staffRestaurantUser);

        return Result.Success();
    }
}
