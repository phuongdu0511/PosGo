namespace PosGo.Contract.Services.V1.Table;

public static class Response
{
    public record TableAreaResponse(
        Guid Id,
        Guid RestaurantId,
        string Name,
        int SortOrder,
        bool IsActive,
        int TableCount
    );

    public record TableResponse(
        Guid Id,
        Guid RestaurantId,
        Guid? AreaId,
        string? AreaName,
        string Name,
        string QrCodeToken,
        int? Seats,
        bool IsActive,
        bool DoNotAllowOrder,
        decimal? MinOrderAmount
    );

    public record TableDetailResponse(
        Guid Id,
        Guid RestaurantId,
        string RestaurantName,
        Guid? AreaId,
        string? AreaName,
        string Name,
        string QrCodeToken,
        int? Seats,
        bool IsActive,
        bool DoNotAllowOrder,
        decimal? MinOrderAmount,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );
}