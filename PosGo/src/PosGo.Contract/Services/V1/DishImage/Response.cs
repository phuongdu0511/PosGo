namespace PosGo.Contract.Services.V1.DishImage;

public static class Response
{
    public record DishImageResponse(
        int Id,
        int DishId,
        string ImageUrl,
        int DisplayOrder,
        bool IsPrimary,
        string? AltText
    );
}