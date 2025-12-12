using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class Dish : SoftDeletableAggregateRoot<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? UnitId { get; private set; }
    public string? Code { get; private set; }
    public Guid? DishTypeId { get; private set; }   // CodeSet=DishType
    public string? ImageUrl { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAvailable { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishCategory? Category { get; private set; }
    public virtual Unit? Unit { get; private set; }
    public virtual CodeItem? DishType { get; private set; }
    public virtual ICollection<DishTranslation> Translations { get; private set; }
    public virtual ICollection<DishVariant> Variants { get; private set; }
    public virtual ICollection<DishSku> Skus { get; private set; }
    public virtual ICollection<DishAttributeGroup> AttributeGroups { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }
}
