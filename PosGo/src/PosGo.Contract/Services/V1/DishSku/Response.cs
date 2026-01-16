namespace PosGo.Contract.Services.V1.DishSku;

public static class Response
{
    public record DishSkuResponse(
        int Id,
        int DishId,
        string DishName,
        string Code,
        decimal Price,
        bool IsDefault,
        int StockQuantity,
        string? ImageUrl,
        bool IsActive,
        decimal? CostPrice,
        List<DishVariantOptionResponse> VariantOptions
    );

    public record DishSkuDetailResponse(
        int Id,
        int DishId,
        string DishName,
        string Code,
        decimal Price,
        bool IsDefault,
        int StockQuantity,
        string? ImageUrl,
        bool IsActive,
        decimal? CostPrice,
        List<DishVariantOptionResponse> VariantOptions,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );

    public record DishVariantOptionResponse(
        int Id,
        int VariantId,
        string VariantCode,
        string VariantName,
        string Code,
        string Name,
        decimal PriceAdjustment
    );
}