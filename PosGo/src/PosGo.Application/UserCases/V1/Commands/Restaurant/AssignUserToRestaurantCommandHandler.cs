using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.Restaurant;

public sealed class AssignUserToRestaurantCommandHandler : ICommandHandler<Command.AssignUserToRestaurantCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    public AssignUserToRestaurantCommandHandler(
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository, 
        IRepositoryBase<Domain.Entities.RestaurantUser, int> restaurantUserRepository, 
        UserManager<Domain.Entities.User> userManager, 
        RoleManager<Domain.Entities.Role> roleManager)
    {
        _restaurantRepository = restaurantRepository;
        _restaurantUserRepository = restaurantUserRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result> Handle(Command.AssignUserToRestaurantCommand request, CancellationToken cancellationToken)
    {
        // 1) Check Restaurant
        var restaurant = await _restaurantRepository.FindByIdAsync(request.RestaurantId) ??
            throw new CommonNotFoundException.CommonException(request.RestaurantId, nameof(Restaurant));

        // 2) Check User
        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? 
            throw new CommonNotFoundException.CommonException(request.UserId, nameof(User));

        // 3) Check Role
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null || role.Scope != SystemConstants.Scope.RESTAURANT)
        {
            throw new CommonNotFoundException.CommonException(request.RoleId, nameof(Role));
        }

        var roleCode = role.RoleCode; // đảm bảo lowercase
        if (roleCode != roleCode.ToLowerInvariant())
            return Result.Failure(new Error("ROLE_CODE_INVALID", "RoleCode phải là chữ thường."));

        var isOwner = roleCode == SystemConstants.RoleCode.OWNER;      // "owner"
        var isManager = roleCode == SystemConstants.RoleCode.MANAGER;  // "manager"
        var isStaff = roleCode == SystemConstants.RoleCode.STAFF;      // "staff"
        var isManagerOrStaff = isManager || isStaff;

        // 4) Nếu gán OWNER: đảm bảo restaurant chỉ có 1 owner
        if (isOwner)
        {
            // restaurant.OwnerUserId đã có người khác => chặn
            if (restaurant.OwnerUserId.HasValue && restaurant.OwnerUserId.Value != request.UserId)
            {
                return Result.Failure(new Error(
                    "RESTAURANT_ALREADY_HAS_OWNER",
                    "Nhà hàng này đã có chủ cửa hàng."));
            }

            // set owner (idempotent)
            restaurant.AssignOwner(request.UserId);
        }

        // 5) Nếu gán MANAGER/STAFF: user chỉ được thuộc 1 restaurant (Active)
        // (Owner có thể thuộc nhiều restaurant => không check đoạn này cho owner)
        if (isManagerOrStaff)
        {
            var activeOtherRestaurant = await _restaurantUserRepository
                .FindAll(x => x.UserId == request.UserId
                           && x.Status == ERestaurantUserStatus.Active
                           && x.RestaurantId != request.RestaurantId)
                .FirstOrDefaultAsync(cancellationToken);

            if (activeOtherRestaurant is not null)
            {
                return Result.Failure(new Error(
                    "USER_ALREADY_ASSIGNED_TO_ANOTHER_RESTAURANT",
                    "Nhân chỉ được gán vào 1 cửa hàng."));
            }
        }

        // 6) Upsert mapping (RestaurantId, UserId)
        var existing = await _restaurantUserRepository.FindAll(
                x => x.RestaurantId == request.RestaurantId
                  && x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is null)
        {
            var ru = Domain.Entities.RestaurantUser.Create(
                restaurantId: request.RestaurantId,
                userId: request.UserId,
                roleId: request.RoleId);

            _restaurantUserRepository.Add(ru);
            return Result.Success();
        }

        // idempotent: nếu role giống + status active => không cần update
        var needChangeRole = existing.RoleId != request.RoleId;
        var needActivate = existing.Status != ERestaurantUserStatus.Active;

        if (!needChangeRole && !needActivate)
            return Result.Success();

        if (needChangeRole)
            existing.ChangeRole(request.RoleId);

        if (needActivate)
            existing.Activate();

        return Result.Success();
    }
}
