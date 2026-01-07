using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishCategory : SoftDeletableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    public string? Code { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishCategory? ParentCategory { get; private set; }
    public virtual ICollection<DishCategory> Children { get; private set; }
    public virtual ICollection<DishCategoryTranslation> Translations { get; private set; }
    public virtual ICollection<Dish> Dishes { get; private set; }
}
