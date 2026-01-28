using PosGo.Domain.Abstractions.Aggregates;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Media : AuditableAggregateRoot<int>, ITenantEntity
{
    public Guid RestaurantId { get; set; }

    // S3 Key (unique identifier trong S3)
    // Format: "{folder}/{restaurantId}/{uuid}.{ext}"
    // Example: "dish/123e4567-e89b-12d3-a456-426614174000/abc123def456.jpeg"
    public string ImageKey { get; private set; }

    // Original filename từ client
    public string FileName { get; private set; }

    // MIME type
    public string ContentType { get; private set; }

    // File size (bytes)
    public long FileSize { get; private set; }
    public string Folder { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public string? Alt { get; private set; }

    // Navigation properties
    public virtual Restaurant Restaurant { get; private set; }

    // Private constructor
    public Media() { }

    // Factory method
    public static Media Create(
        Guid restaurantId,
        string imageKey,
        string fileName,
        string contentType,
        long fileSize,
        string folder,
        int? width,
        int? height,
        string? alt)
    {
        return new Media
        {
            RestaurantId = restaurantId,
            ImageKey = imageKey,
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            Folder = folder,
            Width = width,
            Height = height,
            Alt = alt
        };
    }
}
