using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.DishSku;

public static class Command
{
    public record CreateDishSkuCommand(
        int DishId,
        string Code,
        decimal Price,
        bool IsDefault,
        int StockQuantity,
        string? ImageUrl,
        bool IsActive,
        decimal? CostPrice,
        List<int> VariantOptionIds
    ) : ICommand;

    public record UpdateDishSkuCommand(
        int Id,
        string Code,
        decimal Price,
        bool IsDefault,
        int StockQuantity,
        string? ImageUrl,
        bool IsActive,
        decimal? CostPrice,
        List<int> VariantOptionIds
    ) : ICommand;

    public record DeleteDishSkuCommand(int Id) : ICommand;

    public record UpdateSkuPriceCommand(int Id, decimal Price) : ICommand;

    public record UpdateSkuStockCommand(int Id, int StockQuantity) : ICommand;

    public record UpdateSkuStatusCommand(int Id, bool IsActive) : ICommand;

    public record SetDefaultSkuCommand(int Id) : ICommand;

    // Auto-generate SKUs from variants
    public record GenerateSkusFromVariantsCommand(int DishId) : ICommand;
}