using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantTranslation : Entity<int>
{
    public int VariantId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual DishVariant Variant { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;

    // Private constructor
    private DishVariantTranslation(int variantId, int languageId, string name, string? description)
    {
        VariantId = variantId;
        LanguageId = languageId;
        Name = name.Trim();
        Description = description?.Trim();
    }

    // Factory method
    public static DishVariantTranslation Create(int variantId, int languageId, string name, string? description = null)
        => new DishVariantTranslation(variantId, languageId, name, description);

    // Business methods
    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }
}
