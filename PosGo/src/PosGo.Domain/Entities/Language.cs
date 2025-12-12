using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  CORE / TENANT
// =====================================
public class Language : AuditableEntity<Guid>
{
    public string Code { get; private set; } = null!;   // vi, en...
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public virtual ICollection<Restaurant> DefaultRestaurants { get; private set; }
    public virtual ICollection<RestaurantLanguage> RestaurantLanguages { get; private set; }
    public virtual ICollection<CodeItemTranslation> CodeItemTranslations { get; private set; }
    public virtual ICollection<UnitTranslation> UnitTranslations { get; private set; }
    public virtual ICollection<DishCategoryTranslation> DishCategoryTranslations { get; private set; }
    public virtual ICollection<DishTranslation> DishTranslations { get; private set; }
    public virtual ICollection<DishVariantTranslation> DishVariantTranslations { get; private set; }
    public virtual ICollection<DishVariantOptionTranslation> DishVariantOptionTranslations { get; private set; }
    public virtual ICollection<DishAttributeGroupTranslation> DishAttributeGroupTranslations { get; private set; }
    public virtual ICollection<DishAttributeItemTranslation> DishAttributeItemTranslations { get; private set; }
}
