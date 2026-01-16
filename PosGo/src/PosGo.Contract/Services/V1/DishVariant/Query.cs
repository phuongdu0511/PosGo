using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.DishVariant.Response;

namespace PosGo.Contract.Services.V1.DishVariant;

public static class Query
{
    public record GetDishVariantsQuery(int DishId, string? Language = "vi") : IQuery<List<DishVariantResponse>>;

    public record GetVariantOptionsQuery(int VariantId, string? Language = "vi") : IQuery<List<VariantOptionResponse>>;

    public record GetDishVariantTreeQuery(int DishId, string? Language = "vi") : IQuery<DishVariantTreeResponse>;

    public record GetVariantByIdQuery(int Id, string? Language = "vi") : IQuery<DishVariantDetailResponse>;

    public record GetVariantOptionByIdQuery(int Id, string? Language = "vi") : IQuery<VariantOptionDetailResponse>;
}