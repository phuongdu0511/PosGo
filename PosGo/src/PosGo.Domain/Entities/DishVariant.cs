using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariant : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int DishId { get; private set; }
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishVariantTranslation> Translations { get; private set; } = new List<DishVariantTranslation>();
    public virtual ICollection<DishVariantOption> Options { get; private set; } = new List<DishVariantOption>();

    // Private constructor
    public DishVariant(Guid restaurantId, int dishId, string name, int sortOrder, bool isActive)
    {
        RestaurantId = restaurantId;
        DishId = dishId;
        Name = name;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    // Factory method
    public static DishVariant Create(Guid restaurantId, Dish dish, string name, int sortOrder = 0, bool isActive = true)
    {
        var variant = new DishVariant(restaurantId, dish.Id, name, sortOrder, isActive);
        variant.Dish = dish;
        return variant;
    }

    // Business methods
    public void Update(string name, int sortOrder, bool isActive)
    {
        Name = name;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
