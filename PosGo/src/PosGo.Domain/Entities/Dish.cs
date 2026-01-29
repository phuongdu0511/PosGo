using PosGo.Domain.Abstractions.Aggregates;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class Dish : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int? CategoryId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public int? UnitId { get; private set; }
    public int? DishTypeId { get; private set; }   // CodeSet=DishType
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAvailable { get; private set; }
    public bool ShowOnMenu { get; private set; }
    public decimal? TaxRate { get; private set; }
    public string[] Images { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual DishCategory? Category { get; private set; }
    public virtual Unit? Unit { get; private set; }
    public virtual CodeItem? DishType { get; private set; }
    public virtual ICollection<DishTranslation> Translations { get; private set; }
    public virtual ICollection<DishVariant> Variants { get; private set; }
    public virtual ICollection<DishSku> Skus { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }
    public virtual ICollection<DishComboItem> ComboItems { get; private set; } // Món này là combo
    public virtual ICollection<DishComboItem> UsedInCombos { get; private set; } // Món này được dùng trong combo

    // Private constructor
    public Dish(Guid restaurantId, string name, string? description, int? categoryId, int? unitId, int? dishTypeId, 
        int sortOrder, bool isActive, bool isAvailable, bool showOnMenu, decimal? taxRate, string[]? images = null)
    {
        RestaurantId = restaurantId;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UnitId = unitId;
        DishTypeId = dishTypeId;
        SortOrder = sortOrder;
        IsActive = isActive;
        IsAvailable = isAvailable;
        ShowOnMenu = showOnMenu;
        TaxRate = taxRate;
        Images = images ?? Array.Empty<string>();
    }

    // Factory method
    public static Dish Create(Guid restaurantId, string name, string? description = null, int? categoryId = null, int? unitId = null,
        int? dishTypeId = null, int sortOrder = 0, bool isActive = true, 
        bool isAvailable = true, bool showOnMenu = true, decimal? taxRate = null, string[]? images = null)
        => new Dish(restaurantId, name, description, categoryId, unitId, dishTypeId, sortOrder, 
            isActive, isAvailable, showOnMenu, taxRate, images);

    // Business methods
    public void Update(string name, string? description, int? categoryId, int? unitId, int? dishTypeId, 
        int sortOrder, bool isActive, bool isAvailable, bool showOnMenu, decimal? taxRate)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UnitId = unitId;
        DishTypeId = dishTypeId;
        SortOrder = sortOrder;
        IsActive = isActive;
        IsAvailable = isAvailable;
        ShowOnMenu = showOnMenu;
        TaxRate = taxRate;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetAvailable() => IsAvailable = true;
    public void SetUnavailable() => IsAvailable = false;
    public void ShowOnMenuDisplay() => ShowOnMenu = true;
    public void HideFromMenu() => ShowOnMenu = false;
}
