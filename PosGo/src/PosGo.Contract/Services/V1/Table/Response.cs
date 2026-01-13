namespace PosGo.Contract.Services.V1.Table;

public static class Response
{
    public record TableAreaResponse(
        Guid RestaurantId,
        string Name,
        int SortOrder,
        bool IsActive
    );

    public record TableResponse(
        Guid RestaurantId,
        int? AreaId,
        string? AreaName,
        string Name,
        string QrCodeToken,
        int? Seats,
        bool IsActive,
        bool DoNotAllowOrder,
        decimal? MinOrderAmount
    );

    public record TableDetailResponse(
        Guid RestaurantId,
        string RestaurantName,
        int? AreaId,
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