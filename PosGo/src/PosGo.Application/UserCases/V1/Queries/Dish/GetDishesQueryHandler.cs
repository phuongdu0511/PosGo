using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Dish;

public sealed class GetDishesQueryHandler : IQueryHandler<Query.GetDishesQuery, PagedResult<Response.DishResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IMapper _mapper;

    public GetDishesQueryHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<Response.DishResponse>>> Handle(Query.GetDishesQuery request, CancellationToken cancellationToken)
    {
        // Load all translations - frontend will handle language selection
        IQueryable<Domain.Entities.Dish> dishesQuery = _dishRepository.FindAll()
            .Include(d => d.Category)
            .Include(d => d.Unit)
            .Include(d => d.DishType)
            .Include(d => d.Translations)
            .ThenInclude(t => t.Language);

        if (request.CategoryId.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.CategoryId == request.CategoryId.Value);
        }

        if (request.UnitId.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.UnitId == request.UnitId.Value);
        }

        if (request.DishTypeId.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.DishTypeId == request.DishTypeId.Value);
        }

        if (request.IsActive.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.IsActive == request.IsActive.Value);
        }

        if (request.IsAvailable.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.IsAvailable == request.IsAvailable.Value);
        }

        if (request.ShowOnMenu.HasValue)
        {
            dishesQuery = dishesQuery.Where(d => d.ShowOnMenu == request.ShowOnMenu.Value);
        }

        // Apply sorting
        dishesQuery = request.SortOrder == SortOrder.Descending
            ? dishesQuery.OrderByDescending(GetSortProperty(request))
            : dishesQuery.OrderBy(GetSortProperty(request));

        var dishes = await PagedResult<Domain.Entities.Dish>.CreateAsync(
            dishesQuery, request.PageIndex, request.PageSize);

        var result = _mapper.Map<PagedResult<Response.DishResponse>>(dishes);

        return Result.Success(result);
    }

    private static Expression<Func<Domain.Entities.Dish, object>> GetSortProperty(Query.GetDishesQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "name" => dish => dish.Translations.FirstOrDefault() != null ? dish.Translations.FirstOrDefault()!.Name : "",
            "category" => dish => dish.Category != null ? dish.Category.Translations.FirstOrDefault()!.Name : "",
            "sortorder" => dish => dish.SortOrder,
            "isactive" => dish => dish.IsActive,
            "isavailable" => dish => dish.IsAvailable,
            "showonmenu" => dish => dish.ShowOnMenu,
            "createdat" => dish => dish.CreatedAt,
            _ => dish => dish.SortOrder // Default sort
        };
}