using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  IDENTITY + ROLE
// =====================================
public class UserSystemRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;

    public UserSystemRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserSystemRole Create(Guid userId, Guid roleId)
        => new(userId, roleId);
}
