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
    public int DefaultLanguageId { get; private set; }
    public string? TimeZone { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? OwnerUserId { get; private set; }
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
    public virtual ICollection<Order> Orders { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }
    public virtual ICollection<RestaurantOpeningHour> OpeningHours { get; private set; }
    public virtual ICollection<RestaurantPlan> RestaurantPlans { get; set; }
    public virtual User? OwnerUser { get; private set; }

    public Restaurant(
        Guid id,
        string name,
        string slug,
        int defaultLanguageId,
        string? address,
        string? city,
        string? country,
        string? phone,
        string? timeZone,
        string? logoUrl,
        string? description,
        Guid? restaurantGroupId)
    {
        Id = id;
        Name = name;
        Slug = slug;
        DefaultLanguageId = defaultLanguageId;
        Address = address;
        City = city;
        Country = country;
        Phone = phone;
        TimeZone = timeZone;
        LogoUrl = logoUrl;
        Description = description;
        RestaurantGroupId = restaurantGroupId;
    }

    public static Restaurant Create(
        Guid id,
        string name,
        string slug,
        int defaultLanguageId,
        string? address,
        string? city,
        string? country,
        string? phone,
        string? timeZone,
        string? logoUrl,
        string? description,
        Guid? restaurantGroupId)
    {
        return new Restaurant(
            id,
            name,
            slug,
            defaultLanguageId,
            address,
            city,
            country,
            phone,
            timeZone,
            logoUrl,
            description,
            restaurantGroupId);
    }

    public void Update(
        string name,
        string slug,
        int defaultLanguageId,
        string? address,
        string? city,
        string? country,
        string? phone,
        string? timeZone,
        string? logoUrl,
        string? description,
        Guid? restaurantGroupId,
        bool isActive)
    {
        Name = name;
        Slug = slug;
        DefaultLanguageId = defaultLanguageId;
        Address = address;
        City = city;
        Country = country;
        Phone = phone;
        TimeZone = timeZone;
        LogoUrl = logoUrl;
        Description = description;
        RestaurantGroupId = restaurantGroupId;
        IsActive = isActive;
    }

    public void AssignOwner(Guid userId)
    {
        // idempotent
        if (OwnerUserId == userId) return;

        // nếu đã có owner khác => chặn
        if (OwnerUserId.HasValue && OwnerUserId.Value != userId)
            throw new ArgumentException("Nhà hàng đã có chủ cửa hàng.");

        OwnerUserId = userId;
    }

    public void ClearOwner(Guid userId)
    {
        if (OwnerUserId == userId)
            OwnerUserId = null;
    }
}
