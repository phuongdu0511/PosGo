using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantTranslation : Entity<Guid>
{
    public Guid VariantId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual DishVariant Variant { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
