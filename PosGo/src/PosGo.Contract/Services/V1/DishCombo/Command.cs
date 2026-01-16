using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.DishCombo;

public static class Command
{
    public record AddComboItemCommand(
        int ComboDishId,
        int ItemDishId,
        int? ItemDishSkuId,
        int Quantity,
        bool IsRequired,
        int DisplayOrder
    ) : ICommand;

    public record UpdateComboItemCommand(
        int Id,
        int Quantity,
        bool IsRequired,
        int DisplayOrder,
        int? ItemDishSkuId
    ) : ICommand;

    public record RemoveComboItemCommand(int Id) : ICommand;

    public record ReorderComboItemsCommand(
        int ComboDishId,
        List<ComboItemOrderDto> Items
    ) : ICommand;
}

public record ComboItemOrderDto(
    int Id,
    int DisplayOrder
);