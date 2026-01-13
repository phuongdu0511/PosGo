using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class UnitTranslation : Entity<int>
{
    public int UnitId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;

    public virtual Unit Unit { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
