using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.DishSku.Response;

namespace PosGo.Contract.Services.V1.DishSku;

public static class Query
{
    public record GetDishSkusQuery(int DishId) : IQuery<List<DishSkuResponse>>;

    public record GetSkuByIdQuery(int Id) : IQuery<DishSkuDetailResponse>;

    public record GetSkuByVariantOptionsQuery(
        int DishId,
        List<int> VariantOptionIds
    ) : IQuery<DishSkuResponse>;

    public record GetAvailableSkusQuery(int DishId) : IQuery<List<DishSkuResponse>>;

    public record GetDefaultSkuQuery(int DishId) : IQuery<DishSkuResponse>;
}