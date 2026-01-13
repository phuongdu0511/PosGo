using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  CORE / TENANT
// =====================================
public class Language : AuditableEntity<int>
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

    // Private constructor
    public Language(string code, string name, bool isActive = true)
    {
        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        IsActive = isActive;
    }

    // Factory method
    public static Language Create(string code, string name, bool isActive = true)
        => new Language(code, name, isActive);

    // Business methods
    public void Update(string code, string name, bool isActive)
    {
        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
