namespace PosGo.Contract.Services.V1.DishVariant;

public static class Response
{
    public record DishVariantResponse(
        int Id,
        int DishId,
        string Code,
        string Name,
        int SortOrder,
        bool IsActive,
        List<VariantOptionResponse> Options
    );

    public record DishVariantDetailResponse(
        int Id,
        int DishId,
        string DishName,
        string Code,
        string Name,
        int SortOrder,
        bool IsActive,
        List<VariantOptionResponse> Options,
        List<DishVariantTranslationResponse> Translations,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );

    public record VariantOptionResponse(
        int Id,
        int VariantId,
        string Code,
        string Name,
        int SortOrder,
        bool IsDefault,
        decimal PriceAdjustment,
        bool IsActive
    );

    public record VariantOptionDetailResponse(
        int Id,
        int VariantId,
        string VariantName,
        string Code,
        string Name,
        int SortOrder,
        bool IsDefault,
        decimal PriceAdjustment,
        bool IsActive,
        List<DishVariantOptionTranslationResponse> Translations,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );

    public record DishVariantTreeResponse(
        int DishId,
        string DishName,
        List<DishVariantResponse> Variants
    );

    public record DishVariantTranslationResponse(
        int LanguageId,
        string LanguageCode,
        string LanguageName,
        string Name,
        string? Description
    );

    public record DishVariantOptionTranslationResponse(
        int LanguageId,
        string LanguageCode,
        string LanguageName,
        string Name,
        string? Description
    );
}