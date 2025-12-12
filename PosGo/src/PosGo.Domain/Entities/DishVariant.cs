using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariant : SoftDeletableEntity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid DishId { get; private set; }

    public string Code { get; private set; } = null!;  // SIZE...
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishVariantTranslation> Translations { get; private set; }
    public virtual ICollection<DishVariantOption> Options { get; private set; }
}
