using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ROLE ↔ PERMISSION
// =====================================
public class RolePermission : AuditableEntity<Guid>
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Guid GrantedByUserId { get; private set; }

    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;
    public virtual User GrantedByUser { get; private set; } = null!;

    private RolePermission() { }

    private RolePermission(Guid id, Guid roleId, Guid permissionId, Guid grantedByUserId)
    {
        Id = id;
        RoleId = roleId;
        PermissionId = permissionId;
        GrantedByUserId = grantedByUserId;
    }

    public static RolePermission Grant(Guid roleId, Guid permissionId, Guid grantedByUserId)
        => new(Guid.NewGuid(), roleId, permissionId, grantedByUserId);
}
