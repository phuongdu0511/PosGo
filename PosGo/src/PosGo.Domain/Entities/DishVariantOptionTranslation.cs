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

    public virtual DishVariantOption VariantOption { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
