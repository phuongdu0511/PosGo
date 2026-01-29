using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.DishCategory;

public sealed class GetActiveDishCategoriesQueryHandler : IQueryHandler<Query.GetActiveDishCategoriesQuery, List<Response.DishCategoryResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IMapper _mapper;
    public GetActiveDishCategoriesQueryHandler(
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.DishCategoryResponse>>> Handle(Query.GetActiveDishCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Validate RestaurantId is provided for anonymous access
        if (!request.RestaurantId.HasValue)
        {
            return Result.Failure<List<Response.DishCategoryResponse>>(
                new Error("RESTAURANT_ID_REQUIRED", "Restaurant ID is required."));
        }

        // Load all translations - frontend will handle language selection
        var categories = await _categoryRepository
            .FindAll(c => c.IsActive && c.ShowOnMenu)
            .Include(c => c.Dishes)
            .Include(c => c.Translations)
            .ThenInclude(t => t.Language)
            .AsSplitQuery()
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<Response.DishCategoryResponse>>(categories);

        return Result.Success(result);
    }
}
