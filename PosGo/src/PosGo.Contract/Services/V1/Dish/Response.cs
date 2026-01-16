namespace PosGo.Contract.Services.V1.Dish;

public static class Response
{
    public record DishResponse(
        int Id,
        Guid RestaurantId,
        string Name,
        string? Description,
        int? CategoryId,
        int? UnitId,
        int? DishTypeId,
        int SortOrder,
        bool IsActive,
        bool IsAvailable,
        bool ShowOnMenu,
        decimal? TaxRate,
        List<DishTranslationResponse> Translations,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );

    public record DishDetailResponse(
        int Id,
        Guid RestaurantId,
        string RestaurantName,
        string Name,
        string? Description,
        int? CategoryId,
        string? CategoryName,
        int? UnitId,
        string? UnitName,
        int? DishTypeId,
        string? DishTypeName,
        int SortOrder,
        bool IsActive,
        bool IsAvailable,
        bool ShowOnMenu,
        decimal? TaxRate,
        List<DishTranslationResponse> Translations,
        List<DishVariantResponse> Variants,
        List<DishSkuResponse> Skus,
        List<DishImageResponse> Images,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );

    public record DishTranslationResponse(
        string Language,
        string Name,
        string? Description
    );

    public record DishVariantResponse(
        int Id,
        string Code,
        string Name,
        int SortOrder,
        bool IsActive,
        List<DishVariantOptionResponse> Options
    );

    public record DishVariantOptionResponse(
        int Id,
        string Code,
        string Name,
        int SortOrder,
        bool IsDefault,
        decimal PriceAdjustment,
        bool IsActive
    );

    public record DishSkuResponse(
        int Id,
        string Code,
        decimal Price,
        bool IsDefault,
        int StockQuantity,
        string? ImageUrl,
        bool IsActive,
        decimal? CostPrice,
        List<DishVariantOptionResponse> VariantOptions
    );

    public record DishImageResponse(
        int Id,
        string ImageUrl,
        int DisplayOrder,
        bool IsPrimary,
        string? AltText
    );
}