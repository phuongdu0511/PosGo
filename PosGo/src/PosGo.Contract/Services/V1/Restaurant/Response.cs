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
        Guid DefaultLanguageId,
        string? TimeZone,
        string? LogoUrl,
        string? Description,
        Guid? RestaurantGroupId
    );
}
