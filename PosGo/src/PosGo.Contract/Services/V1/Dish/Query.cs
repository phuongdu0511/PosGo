using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Dish.Response;

namespace PosGo.Contract.Services.V1.Dish;

public static class Query
{
    public record GetDishesQuery(
        string? SearchTerm,
        int? CategoryId,
        int? UnitId,
        int? DishTypeId,
        bool? IsActive,
        bool? IsAvailable,
        bool? ShowOnMenu,
        string? SortColumn,
        SortOrder? SortOrder,
        IDictionary<string, SortOrder>? SortColumnAndOrder,
        int PageIndex,
        int PageSize
    ) : IQuery<PagedResult<DishResponse>>;

    public record GetDishByIdQuery(int Id) : IQuery<DishDetailResponse>;

    public record GetDishByCodeQuery(string Code) : IQuery<DishResponse>;

    public record GetDishesByCategoryQuery(int CategoryId) : IQuery<List<DishResponse>>;

    public record GetActiveMenuDishesQuery(Guid? RestaurantId) : IQuery<List<DishResponse>>;

    public record GetDishTranslationsQuery(int DishId) : IQuery<List<DishTranslationResponse>>;
}