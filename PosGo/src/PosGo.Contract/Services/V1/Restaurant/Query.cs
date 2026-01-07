using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Restaurant.Response;

namespace PosGo.Contract.Services.V1.Restaurant;

public static class Query
{
    public record GetRestaurantsQuery(string? SearchTerm, string? SortColumn, SortOrder? SortOrder, IDictionary<string, SortOrder>? SortColumnAndOrder, int PageIndex, int PageSize) : IQuery<PagedResult<RestaurantResponse>>;
    public record GetRestaurantByIdQuery(Guid Id) : IQuery<RestaurantResponse>;
    public record GetMyRestaurantsQuery() : IQuery<List<MyRestaurantResponse>>;
}
