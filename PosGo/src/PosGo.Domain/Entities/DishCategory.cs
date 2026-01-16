using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishCategory : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool ShowOnMenu { get; private set; } = true;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishCategory? ParentCategory { get; private set; }
    public virtual ICollection<DishCategory> Children { get; private set; }
    public virtual ICollection<DishCategoryTranslation> Translations { get; private set; }
    public virtual ICollection<Dish> Dishes { get; private set; }

    // Private constructor
    public DishCategory(Guid restaurantId, string name, string? description, int? parentCategoryId, string? imageUrl, int sortOrder, bool isActive, bool showOnMenu)
    {
        RestaurantId = restaurantId;
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
        ShowOnMenu = showOnMenu;
    }

    // Factory method
    public static DishCategory Create(Guid restaurantId, string name, string? description = null, int? parentCategoryId = null, string? imageUrl = null, int sortOrder = 0, bool isActive = true, bool showOnMenu = true)
        => new DishCategory(restaurantId, name, description, parentCategoryId, imageUrl, sortOrder, isActive, showOnMenu);

    // Business methods
    public void Update(string name, string? description, int? parentCategoryId, string? imageUrl, int sortOrder, bool isActive, bool showOnMenu)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
        ShowOnMenu = showOnMenu;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void ShowOnMenuDisplay() => ShowOnMenu = true;
    public void HideFromMenu() => ShowOnMenu = false;
}
