using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ORDER / REPORT
// =====================================
public class OrderItemAttribute : AuditableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid OrderItemId { get; private set; }
    public string GroupName { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual OrderItem OrderItem { get; private set; } = null!;
}
