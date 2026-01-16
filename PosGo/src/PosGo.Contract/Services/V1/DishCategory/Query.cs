using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.DishCategory.Response;

namespace PosGo.Contract.Services.V1.DishCategory;

public static class Query
{
    public record GetDishCategoriesQuery(
        string? SearchTerm,
        bool? IsActive,
        string? SortColumn,
        SortOrder? SortOrder,
        IDictionary<string, SortOrder>? SortColumnAndOrder,
        int PageIndex,
        int PageSize
    ) : IQuery<PagedResult<DishCategoryResponse>>;

    public record GetDishCategoryByIdQuery(int Id) : IQuery<DishCategoryDetailResponse>;

    public record GetDishCategoryByCodeQuery(string Code) : IQuery<DishCategoryResponse>;

    public record GetActiveDishCategoriesQuery(Guid? RestaurantId) : IQuery<List<DishCategoryResponse>>;

    public record GetDishCategoryTranslationsQuery(int CategoryId) : IQuery<List<DishCategoryTranslationResponse>>;
}