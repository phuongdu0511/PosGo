using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ATTRIBUTE OPTIONS (không ảnh hưởng giá)
// =====================================
public class DishAttributeItem : SoftDeletableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid AttributeGroupId { get; private set; }

    public string? Code { get; private set; }  // HOT/ICE, 0/30/50...
    public int SortOrder { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishAttributeGroup AttributeGroup { get; private set; } = null!;
    public virtual ICollection<DishAttributeItemTranslation> Translations { get; private set; }
}
