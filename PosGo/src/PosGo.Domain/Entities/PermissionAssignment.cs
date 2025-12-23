using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ROLE A GÁN QUYỀN CHO ROLE B
// =====================================
public class PermissionAssignment : AuditableEntity<Guid>
{
    public Guid FromRoleId { get; private set; }    // Role gán (Admin/Owner)
    public Guid ToRoleId { get; private set; }      // Role nhận (Owner/Manager/Staff)
    public Guid PermissionId { get; private set; }
    public Guid AssignedByUserId { get; private set; }

    public virtual Role FromRole { get; private set; } = null!;
    public virtual Role ToRole { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;
    public virtual User AssignedByUser { get; private set; } = null!;

    private PermissionAssignment() { }

    private PermissionAssignment(
        Guid id,
        Guid fromRoleId,
        Guid toRoleId,
        Guid permissionId,
        Guid assignedByUserId)
    {
        Id = id;
        FromRoleId = fromRoleId;
        ToRoleId = toRoleId;
        PermissionId = permissionId;
        AssignedByUserId = assignedByUserId;
    }

    public static PermissionAssignment Assign(
        Guid fromRoleId,
        Guid toRoleId,
        Guid permissionId,
        Guid assignedByUserId)
        => new(Guid.NewGuid(), fromRoleId, toRoleId, permissionId, assignedByUserId);
}
