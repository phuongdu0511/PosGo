using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
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
        var restaurant = await _restaurantRepository.FindByIdAsync(request.RestaurantId) ??
            throw new CommonNotFoundException.CommonException(request.RestaurantId, nameof(Restaurant));

        var user = await _userManager.FindByIdAsync(request.UserId.ToString()) ?? 
            throw new CommonNotFoundException.CommonException(request.UserId, nameof(User));

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null || role.Scope != SystemConstants.Scope.RESTAURANT)
        {
            throw new CommonNotFoundException.CommonException(request.RoleId, nameof(Role));
        }

        // 4. Tìm mapping (Restaurant, User) hiện tại
        var existing = await _restaurantUserRepository.FindSingleAsync(
            x => x.RestaurantId == request.RestaurantId
              && x.UserId == request.UserId,
            cancellationToken);

        if (existing is null)
        {
            // 4a. Chưa có => tạo mới
            var ru = Domain.Entities.RestaurantUser.Create(
                restaurantId: request.RestaurantId,
                userId: request.UserId,
                roleId: request.RoleId);

            _restaurantUserRepository.Add(ru);
        }
        else
        {
            // 4b. Đã có => cập nhật role + bật lại
            existing.ChangeRole(request.RoleId);
            existing.Activate();
        }

        return Result.Success();
    }
}
