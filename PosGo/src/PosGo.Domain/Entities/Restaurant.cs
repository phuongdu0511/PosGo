using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  CORE / TENANT
// =====================================
public class Restaurant : SoftDeletableAggregateRoot<Guid>
{
    public Guid? RestaurantGroupId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public string? Phone { get; private set; }
    public Guid DefaultLanguageId { get; private set; }
    public string? TimeZone { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Description { get; private set; }
    public virtual RestaurantGroup? RestaurantGroup { get; private set; }
    public virtual Language DefaultLanguage { get; private set; } = null!;
    public virtual ICollection<RestaurantLanguage> RestaurantLanguages { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }
    public virtual ICollection<TableArea> TableAreas { get; private set; }
    public virtual ICollection<Table> Tables { get; private set; }
    public virtual ICollection<Unit> Units { get; private set; }
    public virtual ICollection<DishCategory> DishCategories { get; private set; }
    public virtual ICollection<Dish> Dishes { get; private set; }
    public virtual ICollection<DishVariant> DishVariants { get; private set; }
    public virtual ICollection<DishVariantOption> DishVariantOptions { get; private set; }
    public virtual ICollection<DishSku> DishSkus { get; private set; }
    public virtual ICollection<DishAttributeGroup> DishAttributeGroups { get; private set; }
    public virtual ICollection<DishAttributeItem> DishAttributeItems { get; private set; }
    public virtual ICollection<Order> Orders { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }
    public virtual ICollection<OrderItemAttribute> OrderItemAttributes { get; private set; }
    public virtual ICollection<RestaurantOpeningHour> OpeningHours { get; private set; }
}
