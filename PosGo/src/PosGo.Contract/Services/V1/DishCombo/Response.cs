namespace PosGo.Contract.Services.V1.DishCombo;

public static class Response
{
    public record ComboItemResponse(
        int Id,
        int ComboDishId,
        int ItemDishId,
        string ItemDishName,
        int? ItemDishSkuId,
        string? ItemDishSkuCode,
        decimal? ItemDishSkuPrice,
        int Quantity,
        bool IsRequired,
        int DisplayOrder
    );

    public record ComboDetailResponse(
        int ComboDishId,
        string ComboDishName,
        List<ComboItemResponse> Items,
        decimal TotalPrice
    );
}