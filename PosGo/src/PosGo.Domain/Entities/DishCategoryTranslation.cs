using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  MENU: UNIT / CATEGORY / DISH
// =====================================
public class DishCategoryTranslation : Entity<int>
{
    public int CategoryId { get; private set; }
    public int LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public virtual DishCategory Category { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;

    // Private constructor
    public DishCategoryTranslation(int categoryId, int languageId, string name, string? description)
    {
        CategoryId = categoryId;
        LanguageId = languageId;
        Name = name.Trim();
        Description = description?.Trim();
    }

    // Factory method with CategoryId
    public static DishCategoryTranslation Create(int categoryId, int languageId, string name, string? description = null)
        => new DishCategoryTranslation(categoryId, languageId, name, description);

    // Factory method with Category navigation property (for new entities without ID)
    public static DishCategoryTranslation Create(DishCategory category, int languageId, string name, string? description = null)
    {
        var translation = new DishCategoryTranslation(0, languageId, name, description);
        translation.Category = category;
        return translation;
    }

    // Business methods
    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }
}
