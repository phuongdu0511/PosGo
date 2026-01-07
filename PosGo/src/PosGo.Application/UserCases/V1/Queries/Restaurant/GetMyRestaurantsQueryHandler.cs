using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Restaurant;

public sealed class GetMyRestaurantsQueryHandler : IQueryHandler<Query.GetMyRestaurantsQuery, List<Response.MyRestaurantResponse>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IMapper _mapper;

    public GetMyRestaurantsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IRepositoryBase<Domain.Entities.RestaurantUser, int> restaurantUserRepository,
        IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _restaurantUserRepository = restaurantUserRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.MyRestaurantResponse>>> Handle(Query.GetMyRestaurantsQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy userId từ HttpContext
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();

        // 2. Lấy danh sách RestaurantUser Active của user
        var restaurantUsers = await _restaurantUserRepository
            .FindAll(x => x.UserId == userId && x.Status == ERestaurantUserStatus.Active)
            .ToListAsync(cancellationToken);

        if (!restaurantUsers.Any())
        {
            return Result.Success(new List<Response.MyRestaurantResponse>());
        }

        // 3. Lấy danh sách RestaurantId
        var restaurantIds = restaurantUsers
            .Select(x => x.RestaurantId)
            .Distinct()
            .ToList();

        // 4. Query restaurants
        var restaurants = await _restaurantRepository
            .FindAll(x => restaurantIds.Contains(x.Id))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        // 5. Map sang Response
        var result = _mapper.Map<List<Response.MyRestaurantResponse>>(restaurants);

        return Result.Success(result);
    }
}
