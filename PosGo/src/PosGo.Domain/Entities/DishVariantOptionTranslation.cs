using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantOptionTranslation : Entity<int>
{
    public int VariantOptionId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual DishVariantOption VariantOption { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;

    // Private constructor
    private DishVariantOptionTranslation(int variantOptionId, int languageId, string name, string? description)
    {
        VariantOptionId = variantOptionId;
        LanguageId = languageId;
        Name = name.Trim();
        Description = description?.Trim();
    }

    // Factory method
    public static DishVariantOptionTranslation Create(int variantOptionId, int languageId, string name, string? description = null)
        => new DishVariantOptionTranslation(variantOptionId, languageId, name, description);

    // Business methods
    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }
}
