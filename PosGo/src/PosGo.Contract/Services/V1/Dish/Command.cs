using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Dish;

public static class Command
{
    public record CreateDishCommand(
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
        List<DishTranslationDto>? Translations
    ) : ICommand;

    public record UpdateDishCommand(
        int Id,
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
        List<DishTranslationDto>? Translations
    ) : ICommand;

    public record DeleteDishCommand(int Id) : ICommand;

    public record UpdateDishStatusCommand(int Id, bool IsActive) : ICommand;

    public record UpdateDishAvailabilityCommand(int Id, bool IsAvailable) : ICommand;

    public record UpdateDishMenuVisibilityCommand(int Id, bool ShowOnMenu) : ICommand;

    public record UpdateDishTranslationsCommand(
        int Id,
        List<DishTranslationDto> Translations
    ) : ICommand;
}

public record DishTranslationDto(
    int LanguageId,
    string Name,
    string? Description
);