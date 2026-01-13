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

    public virtual DishVariant Variant { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
