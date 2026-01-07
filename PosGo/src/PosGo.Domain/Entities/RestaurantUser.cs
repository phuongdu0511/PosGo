using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  IDENTITY + ROLE
// =====================================
public class RestaurantUser : AuditableEntity<int>, ITenantEntity
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
    public RestaurantUser(
        Guid restaurantId,
        Guid userId,
        Guid roleId,
        ERestaurantUserStatus status)
    {
        RestaurantId = restaurantId;
        UserId = userId;
        RoleId = roleId;
        Status = status;
    }

    public static RestaurantUser Create(
        Guid restaurantId,
        Guid userId,
        Guid roleId)
        => new(
            restaurantId: restaurantId,
            userId: userId,
            roleId: roleId,
            status: ERestaurantUserStatus.Active);

    public void ChangeRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public void Activate()
    {
        Status = ERestaurantUserStatus.Active;
    }

    public void Deactivate()
    {
        Status = ERestaurantUserStatus.Blocked;
    }
}
