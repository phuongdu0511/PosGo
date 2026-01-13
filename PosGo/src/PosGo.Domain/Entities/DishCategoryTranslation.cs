using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishCategoryTranslation : AuditableEntity<int>
{
    public int CategoryId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual DishCategory Category { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
