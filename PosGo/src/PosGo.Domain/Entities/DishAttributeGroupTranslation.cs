using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ATTRIBUTE OPTIONS (không ảnh hưởng giá)
// =====================================
public class DishAttributeGroupTranslation : Entity<Guid>
{
    public Guid AttributeGroupId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual DishAttributeGroup AttributeGroup { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
