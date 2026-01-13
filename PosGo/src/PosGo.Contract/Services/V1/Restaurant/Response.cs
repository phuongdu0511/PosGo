namespace PosGo.Contract.Services.V1.Restaurant;

public static class Response
{
    public record RestaurantResponse(
        Guid Id,
        string Name,
        string Slug,
        string? Address,
        string? City,
        string? Country,
        string? Phone,
        int DefaultLanguageId,
        string? TimeZone,
        string? LogoUrl,
        string? Description,
        Guid? RestaurantGroupId,
        bool IsActive
    );

    public record MyRestaurantResponse(
        Guid Id,
        string Name,
        string Slug,
        string? LogoUrl,
        bool IsActive
    );
}
