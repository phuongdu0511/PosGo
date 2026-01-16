namespace PosGo.Contract.Services.V1.DishCategory;

public static class Response
{
    public record DishCategoryResponse(
        int Id,
        Guid RestaurantId,
        string Name,
        string? Description,
        int? ParentCategoryId,
        string? ImageUrl,
        int SortOrder,
        bool IsActive,
        List<DishCategoryTranslationResponse> Translations
    );

    public record DishCategoryDetailResponse(
        int Id,
        Guid RestaurantId,
        string RestaurantName,
        string Name,
        string? Description,
        int? ParentCategoryId,
        string? ParentCategoryName,
        string? ImageUrl,
        int SortOrder,
        bool IsActive,
        int DishCount,
        List<DishCategoryTranslationResponse> Translations
    );

    public record DishCategoryTranslationResponse(
        string Language,
        string Name,
        string? Description
    );
}