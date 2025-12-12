using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  TABLE / QR
// =====================================
public class TableArea : SoftDeletableEntity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public string Name { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual ICollection<Table> Tables { get; private set; }
}
