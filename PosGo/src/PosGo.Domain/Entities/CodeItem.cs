using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  CODE SERVICE
// =====================================
public class CodeItem : AuditableEntity<Guid>
{
    public Guid CodeSetId { get; private set; }

    public string Code { get; private set; } = null!;
    public int? NumericValue { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public virtual CodeSet CodeSet { get; private set; } = null!;
    public virtual ICollection<CodeItemTranslation> Translations { get; private set; }
    public virtual ICollection<Table> TablesUsingStatus { get; private set; }
    public virtual ICollection<Dish> DishesUsingType { get; private set; }
    public virtual ICollection<Order> OrdersUsingStatus { get; private set; }
}
