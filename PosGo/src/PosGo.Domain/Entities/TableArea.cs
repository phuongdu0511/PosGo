using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  TABLE / QR
// =====================================
public class TableArea : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public string Name { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual ICollection<Table> Tables { get; private set; }

    public TableArea(Guid restaurantId, string name, int sortOrder, bool isActive = true)
    {
        RestaurantId = restaurantId;
        Name = name;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public static TableArea Create(Guid restaurantId, string name, int sortOrder, bool isActive = true)
    {
        return new TableArea(restaurantId, name, sortOrder, isActive);
    }

    public void Update(string name, int sortOrder, bool isActive)
    {
        Name = name.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
    }
}
