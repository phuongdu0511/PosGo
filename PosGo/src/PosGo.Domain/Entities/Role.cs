using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  IDENTITY + ROLE
// =====================================
public class Role : AuditableAggregateRoot<Guid>
{
    public string Scope { get; private set; } = null!;  // SYSTEM | RESTAURANT
    public string Code { get; private set; } = null!;   // SystemAdmin, Owner, Manager...
    public string Name { get; private set; } = null!;
    public int Rank { get; private set; }
    public bool IsActive { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }

    // RBAC
    public virtual ICollection<RolePermission> RolePermissions { get; private set; }
    public virtual ICollection<PermissionAssignment> FromRolePermissionAssignments { get; private set; }
    public virtual ICollection<PermissionAssignment> ToRolePermissionAssignments { get; private set; }
}
