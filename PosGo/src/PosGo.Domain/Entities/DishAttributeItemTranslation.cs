using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ATTRIBUTE OPTIONS (không ảnh hưởng giá)
// =====================================
public class DishAttributeItemTranslation : AuditableEntity<Guid>
{
    public Guid AttributeItemId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual DishAttributeItem AttributeItem { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
