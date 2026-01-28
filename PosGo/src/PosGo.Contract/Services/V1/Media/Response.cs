namespace PosGo.Contract.Services.V1.Media;

public static class Response
{
    public record MediaResponse(
        int Id,
        Guid RestaurantId,
        string ImageKey,
        string FileName,
        string ContentType,
        long FileSize,
        string Folder,
        int? Width,
        int? Height,
        string? Alt,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt
    );
}
