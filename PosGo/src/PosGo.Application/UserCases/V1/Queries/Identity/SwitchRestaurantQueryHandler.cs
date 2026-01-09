using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Identity;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Helpers;
using static PosGo.Domain.Utilities.Helpers.HttpContextHelper;

namespace PosGo.Application.UserCases.V1.Queries.Identity;

public class SwitchRestaurantQueryHandler : IQueryHandler<Query.SwitchRestaurant, Response.Authenticated>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SwitchRestaurantQueryHandler(
        UserManager<Domain.Entities.User> userManager,
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository,
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IJwtTokenService jwtTokenService,
        ICacheService cacheService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _restaurantRepository = restaurantRepository;
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.SwitchRestaurant request, CancellationToken cancellationToken)
    {
        _httpContextAccessor.HttpContext!.Items[TenantFilterBypass.Key] = true;
        try
        {
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
            var scope = _httpContextAccessor.HttpContext.GetScope();

            var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                throw new CommonNotFoundException.CommonException(userId, nameof(User));

            var restaurant = await _restaurantRepository.FindByIdAsync(request.RestaurantId) ??
                throw new CommonNotFoundException.CommonException(request.RestaurantId, nameof(Restaurant));

            // Check user có thuộc nhà hàng này không
            var ru = await _restaurantUserRepository.FindSingleAsync(
                x => x.RestaurantId == request.RestaurantId
                  && x.UserId == userId
                  && x.Status == ERestaurantUserStatus.Active,
                cancellationToken);

            if (ru is null)
            {
                return Result.Failure<Response.Authenticated>(
                    new Error("FORBIDDEN", "Bạn không có quyền truy cập nhà hàng này."));
            }

            // 5. Generate access token mới gắn restaurant_id
            var accessToken = await _jwtTokenService.GenerateAccessTokenForRestaurantAsync(user, scope, request.RestaurantId);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var response = new Response.Authenticated
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };

            await _cacheService.SetAsync(user.UserName!, response, cancellationToken);

            return Result.Success(response);
        }
        finally
        {
            _httpContextAccessor.HttpContext!.Items.Remove(TenantFilterBypass.Key);
        }
    }
}
