using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class UnitTranslation : Entity<Guid>
{
    public Guid UnitId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual Unit Unit { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
