using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Media;

public static class Command
{
    /// <summary>
    /// Request tạo pre-signed URL để upload ảnh
    /// </summary>
    public record GeneratePresignedUrlCommand(
        string FileName,        // "image.jpeg"
        string ContentType,      // "image/jpeg"
        string? Folder = null     // "dish", "category", "restaurant" - optional, default "media"
    ) : ICommand<GeneratePresignedUrlResponse>;
}

public record GeneratePresignedUrlResponse(
    string ImageKey,      // "dish/uuid.jpeg"
    string Url    // Pre-signed PUT URL
);
