using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ATTRIBUTE OPTIONS (không ảnh hưởng giá)
// =====================================
public class DishAttributeGroup : SoftDeletableEntity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid DishId { get; private set; }

    public string? Code { get; private set; }       // TEMP, SUGAR...
    public bool IsRequired { get; private set; }
    public bool IsMultipleSelection { get; private set; }
    public int? MinSelected { get; private set; }
    public int? MaxSelected { get; private set; }

    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishAttributeGroupTranslation> Translations { get; private set; }
    public virtual ICollection<DishAttributeItem> Items { get; private set; }
}
