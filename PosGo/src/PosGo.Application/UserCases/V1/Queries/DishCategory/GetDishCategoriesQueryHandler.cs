using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.DishCategory;

public sealed class GetDishCategoriesQueryHandler : IQueryHandler<Query.GetDishCategoriesQuery, PagedResult<Response.DishCategoryResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IMapper _mapper;

    public GetDishCategoriesQueryHandler(
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<Response.DishCategoryResponse>>> Handle(Query.GetDishCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Load all translations - frontend will handle language selection
        IQueryable<Domain.Entities.DishCategory> categoriesQuery = _categoryRepository.FindAll()
            .Include(c => c.Translations)
            .ThenInclude(t => t.Language);

        if (request.IsActive.HasValue)
        {
            categoriesQuery = categoriesQuery.Where(c => c.IsActive == request.IsActive.Value);
        }

        // Apply sorting
        categoriesQuery = request.SortOrder == SortOrder.Descending
            ? categoriesQuery.OrderByDescending(GetSortProperty(request))
            : categoriesQuery.OrderBy(GetSortProperty(request));

        var categories = await PagedResult<Domain.Entities.DishCategory>.CreateAsync(
            categoriesQuery, request.PageIndex, request.PageSize);

        var result = _mapper.Map<PagedResult<Response.DishCategoryResponse>>(categories);

        return Result.Success(result);
    }

    private static Expression<Func<Domain.Entities.DishCategory, object>> GetSortProperty(Query.GetDishCategoriesQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "name" => category => category.Name,
            "sortorder" => category => category.SortOrder,
            "createdat" => category => category.CreatedAt,
            _ => category => category.SortOrder // Default sort
        };
}
