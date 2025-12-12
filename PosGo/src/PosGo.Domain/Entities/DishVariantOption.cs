using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantOption : SoftDeletableEntity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid VariantId { get; private set; }

    public string Code { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishVariant Variant { get; private set; } = null!;
    public virtual ICollection<DishVariantOptionTranslation> Translations { get; private set; }
    public virtual ICollection<DishSkuVariantOption> DishSkuVariantOptions { get; private set; }
}
