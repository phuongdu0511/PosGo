using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  IDENTITY + ROLE
// =====================================
public class RestaurantUser : AuditableEntity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public ERestaurantUserStatus Status { get; private set; } = ERestaurantUserStatus.Active;
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;
    public virtual User? CreatedByUser { get; private set; }
    public virtual User? UpdatedByUser { get; private set; }
}
