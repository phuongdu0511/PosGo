using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishTranslation : AuditableEntity<Guid>
{
    public Guid DishId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual Dish Dish { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
