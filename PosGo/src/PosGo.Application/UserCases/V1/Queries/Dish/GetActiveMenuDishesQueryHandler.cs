using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Dish;

public sealed class GetActiveMenuDishesQueryHandler : IQueryHandler<Query.GetActiveMenuDishesQuery, List<Response.DishResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IMapper _mapper;

    public GetActiveMenuDishesQueryHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.DishResponse>>> Handle(Query.GetActiveMenuDishesQuery request, CancellationToken cancellationToken)
    {
        // Validate RestaurantId is provided for anonymous access
        if (!request.RestaurantId.HasValue)
        {
            return Result.Failure<List<Response.DishResponse>>(
                new Error("RESTAURANT_ID_REQUIRED", "Restaurant ID is required."));
        }

        // Load all translations - frontend will handle language selection
        var dishes = await _dishRepository
            .FindAll(d => d.RestaurantId == request.RestaurantId.Value && 
                         d.IsActive && 
                         d.IsAvailable && 
                         d.ShowOnMenu)
            .Include(d => d.Category)
            .Include(d => d.Unit)
            .Include(d => d.DishType)
            .Include(d => d.Translations)
            .ThenInclude(t => t.Language)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<Response.DishResponse>>(dishes);

        return Result.Success(result);
    }
}
