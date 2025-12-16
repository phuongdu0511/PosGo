using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Restaurant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Restaurant;

public sealed class GetRestaurantsQueryHandler : IQueryHandler<Query.GetRestaurantsQuery, PagedResult<Response.RestaurantResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Restaurant, Guid> _restaurantRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public GetRestaurantsQueryHandler(IRepositoryBase<Domain.Entities.Restaurant, Guid> restaurantRepository,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _context = context;
    }
    public async Task<Result<PagedResult<Response.RestaurantResponse>>> Handle(Query.GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        if (request.SortColumnAndOrder.Any()) // =>>  Raw Query when order by multi column
        {
            var PageIndex = request.PageIndex <= 0 ? PagedResult<Domain.Entities.Restaurant>.DefaultPageIndex : request.PageIndex;
            var PageSize = request.PageSize <= 0
                ? PagedResult<Domain.Entities.Restaurant>.DefaultPageSize
                : request.PageSize > PagedResult<Domain.Entities.Restaurant>.UpperPageSize
                ? PagedResult<Domain.Entities.Restaurant>.UpperPageSize : request.PageSize;

            // ============================================
            var restaurantsQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? @$"SELECT * FROM {nameof(Domain.Entities.Restaurant)} ORDER BY "
                : @$"SELECT * FROM {nameof(Domain.Entities.Restaurant)}
                        WHERE {nameof(Domain.Entities.Restaurant.Name)} LIKE '%{request.SearchTerm}%'
                        ORDER BY ";

            foreach (var item in request.SortColumnAndOrder)
                restaurantsQuery += item.Value == SortOrder.Descending
                    ? $"{item.Key} DESC, "
                    : $"{item.Key} ASC, ";

            restaurantsQuery = restaurantsQuery.Remove(restaurantsQuery.Length - 2);

            restaurantsQuery += $" OFFSET {(PageIndex - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";

            var restaurants = await _context.Restaurants.FromSqlRaw(restaurantsQuery)
                .ToListAsync(cancellationToken: cancellationToken);

            var totalCount = await _context.Restaurants.CountAsync(cancellationToken);

            var restaurantPagedResult = PagedResult<Domain.Entities.Restaurant>.Create(restaurants,
                PageIndex,
                PageSize,
                totalCount);

            var result = _mapper.Map<PagedResult<Response.RestaurantResponse>>(restaurantPagedResult);

            return Result.Success(result);
        }
        else // =>> Entity Framework
        {
            var restaurantsQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? _restaurantRepository.FindAll()
            : _restaurantRepository.FindAll(x => x.Name.Contains(request.SearchTerm));

            restaurantsQuery = request.SortOrder == SortOrder.Descending
            ? restaurantsQuery.OrderByDescending(GetSortProperty(request))
            : restaurantsQuery.OrderBy(GetSortProperty(request));

            var restaurants = await PagedResult<Domain.Entities.Restaurant>.CreateAsync(restaurantsQuery,
                request.PageIndex,
                request.PageSize);

            var result = _mapper.Map<PagedResult<Response.RestaurantResponse>>(restaurants);
            return Result.Success(result);
        }
    }

    private static Expression<Func<Domain.Entities.Restaurant, object>> GetSortProperty(Query.GetRestaurantsQuery request)
         => request.SortColumn?.ToLower() switch
         {
             "name" => restaurant => restaurant.Name,
             _ => restaurant => restaurant.CreatedAt // Default Sort Descending on CreatedDate column
         };
}
