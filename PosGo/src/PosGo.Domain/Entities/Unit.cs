using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class Unit : SoftDeletableEntity<int>
{
    public Guid? RestaurantId { get; private set; }   // null = global unit
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant? Restaurant { get; private set; }
    public virtual ICollection<UnitTranslation> Translations { get; private set; }
    public virtual ICollection<Dish> Dishes { get; private set; }
}
