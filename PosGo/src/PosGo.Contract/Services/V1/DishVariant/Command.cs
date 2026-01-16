using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.DishVariant;

public static class Command
{
    // Variant Management
    public record CreateDishVariantCommand(
        int DishId,
        string Code,
        int SortOrder,
        bool IsActive,
        List<DishVariantTranslationDto> Translations
    ) : ICommand;

    public record UpdateDishVariantCommand(
        int Id,
        string Code,
        int SortOrder,
        bool IsActive,
        List<DishVariantTranslationDto> Translations
    ) : ICommand;

    public record DeleteDishVariantCommand(int Id) : ICommand;

    public record UpdateDishVariantStatusCommand(int Id, bool IsActive) : ICommand;

    // Variant Option Management
    public record CreateVariantOptionCommand(
        int VariantId,
        string Code,
        int SortOrder,
        bool IsDefault,
        decimal PriceAdjustment,
        bool IsActive,
        List<DishVariantOptionTranslationDto> Translations
    ) : ICommand;

    public record UpdateVariantOptionCommand(
        int Id,
        string Code,
        int SortOrder,
        bool IsDefault,
        decimal PriceAdjustment,
        bool IsActive,
        List<DishVariantOptionTranslationDto> Translations
    ) : ICommand;

    public record DeleteVariantOptionCommand(int Id) : ICommand;

    public record UpdateVariantOptionStatusCommand(int Id, bool IsActive) : ICommand;

    // Bulk Operations
    public record CreateDishWithVariantsCommand(
        int? CategoryId,
        int? UnitId,
        string? Code,
        int? DishTypeId,
        string? ImageUrl,
        int SortOrder,
        bool IsActive,
        bool IsAvailable,
        bool ShowOnMenu,
        decimal? TaxRate,
        List<DishTranslationDto> DishTranslations,
        List<CreateDishVariantDto> Variants
    ) : ICommand;
}

public record DishVariantTranslationDto(
    int LanguageId,
    string Name,
    string? Description
);

public record DishVariantOptionTranslationDto(
    int LanguageId,
    string Name,
    string? Description
);

public record DishTranslationDto(
    int LanguageId,
    string Name,
    string? Description
);

public record CreateDishVariantDto(
    string Code,
    int SortOrder,
    bool IsActive,
    List<DishVariantTranslationDto> Translations,
    List<CreateVariantOptionDto> Options
);

public record CreateVariantOptionDto(
    string Code,
    int SortOrder,
    bool IsDefault,
    decimal PriceAdjustment,
    bool IsActive,
    List<DishVariantOptionTranslationDto> Translations
);