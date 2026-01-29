using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantOption : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int VariantId { get; private set; }
    public string Value { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;
    public decimal PriceAdjustment { get; private set; } = 0;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishVariant Variant { get; private set; } = null!;
    public virtual ICollection<DishVariantOptionTranslation> Translations { get; private set; }
    public virtual ICollection<DishSkuVariantOption> DishSkuVariantOptions { get; private set; }

    // Private constructor
    public DishVariantOption(Guid restaurantId, int variantId, string value, int sortOrder, decimal priceAdjustment, bool isActive)
    {
        RestaurantId = restaurantId;
        VariantId = variantId;
        Value = value;
        SortOrder = sortOrder;
        PriceAdjustment = priceAdjustment;
        IsActive = isActive;
    }

    // Factory method
    public static DishVariantOption Create(Guid restaurantId, int variantId, string value, int sortOrder = 0, decimal priceAdjustment = 0, bool isActive = true)
        => new DishVariantOption(restaurantId, variantId, value, sortOrder, priceAdjustment, isActive);

    // Business methods
    public void Update(string value, int sortOrder, bool isDefault, decimal priceAdjustment, bool isActive)
    {
        Value = value;
        SortOrder = sortOrder;
        IsDefault = isDefault;
        PriceAdjustment = priceAdjustment;
        IsActive = isActive;
    }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
