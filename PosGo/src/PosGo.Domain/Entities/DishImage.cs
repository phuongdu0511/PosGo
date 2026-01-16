using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class DishImage : Entity<int>
{
    public int DishId { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? AltText { get; private set; }

    // Private constructor
    private DishImage(int dishId, string imageUrl, int displayOrder, bool isPrimary, string? altText)
    {
        DishId = dishId;
        ImageUrl = imageUrl.Trim();
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        AltText = altText?.Trim();
    }

    // Factory method
    public static DishImage Create(int dishId, string imageUrl, int displayOrder = 0, bool isPrimary = false, string? altText = null)
        => new DishImage(dishId, imageUrl, displayOrder, isPrimary, altText);

    // Business methods
    public void Update(string imageUrl, int displayOrder, bool isPrimary, string? altText)
    {
        ImageUrl = imageUrl.Trim();
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        AltText = altText?.Trim();
    }

    public void SetAsPrimary() => IsPrimary = true;
    public void UnsetPrimary() => IsPrimary = false;
}
