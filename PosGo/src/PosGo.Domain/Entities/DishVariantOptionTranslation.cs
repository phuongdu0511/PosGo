using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishVariantOptionTranslation : Entity<Guid>
{
    public Guid VariantOptionId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual DishVariantOption VariantOption { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
