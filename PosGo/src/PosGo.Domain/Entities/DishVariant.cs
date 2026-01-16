using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariant : SoftDeletableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int DishId { get; private set; }

    public string Code { get; private set; } = null!;  // SIZE...
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishVariantTranslation> Translations { get; private set; }
    public virtual ICollection<DishVariantOption> Options { get; private set; }

    // Private constructor
    private DishVariant(Guid restaurantId, int dishId, string code, int sortOrder, bool isActive)
    {
        RestaurantId = restaurantId;
        DishId = dishId;
        Code = code.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    // Factory method
    public static DishVariant Create(Guid restaurantId, int dishId, string code, int sortOrder = 0, bool isActive = true)
        => new DishVariant(restaurantId, dishId, code, sortOrder, isActive);

    // Business methods
    public void Update(string code, int sortOrder, bool isActive)
    {
        Code = code.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
