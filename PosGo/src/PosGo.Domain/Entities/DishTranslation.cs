using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishTranslation : Entity<int>
{
    public int DishId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual Dish Dish { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;

    // Private constructor
    public DishTranslation(int dishId, int languageId, string name, string? description)
    {
        DishId = dishId;
        LanguageId = languageId;
        Name = name;
        Description = description;
    }

    // Factory method with DishId
    public static DishTranslation Create(int dishId, int languageId, string name, string? description = null)
        => new DishTranslation(dishId, languageId, name, description);

    // Factory method with Dish navigation property (for new entities without ID)
    public static DishTranslation Create(Dish dish, int languageId, string name, string? description = null)
    {
        var translation = new DishTranslation(0, languageId, name, description);
        translation.Dish = dish;
        return translation;
    }

    // Business methods
    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
