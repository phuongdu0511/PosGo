using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.DishCategory;

public static class Command
{
    public record CreateDishCategoryCommand(
        string Name,
        string? Description,
        int? ParentCategoryId,
        string? ImageUrl,
        int SortOrder,
        bool IsActive,
        bool ShowOnMenu,
        List<DishCategoryTranslationDto>? Translations
    ) : ICommand;

    public record UpdateDishCategoryCommand(
        int Id,
        string Name,
        string? Description,
        int? ParentCategoryId,
        string? ImageUrl,
        int SortOrder,
        bool IsActive,
        bool ShowOnMenu,
        List<DishCategoryTranslationDto>? Translations
    ) : ICommand;

    public record DeleteDishCategoryCommand(int Id) : ICommand;

    public record UpdateDishCategoryStatusCommand(int Id, bool IsActive) : ICommand;

    public record UpdateDishCategoryTranslationsCommand(
        int Id,
        List<DishCategoryTranslationDto> Translations
    ) : ICommand;
}

public record DishCategoryTranslationDto(
    int LanguageId,
    string Name,
    string? Description
);