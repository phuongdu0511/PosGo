using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  IDENTITY + ROLE
// =====================================
public class User : AuditableAggregateRoot<Guid>
{
    public string UserName { get; private set; } = null!;
    public string Password { get; private set; } = null!;

    public string? FullName { get; private set; }
    public string? Phone { get; private set; }
    public EUserStatus Status { get; private set; } = EUserStatus.Active;
    public virtual User? CreatedByUser { get; private set; }
    public virtual User? UpdatedByUser { get; private set; }
    public virtual ICollection<User> CreatedUsers { get; private set; }
    public virtual ICollection<User> UpdatedUsers { get; private set; }

    public virtual ICollection<UserSystemRole> SystemRoles { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersCreated { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersUpdated { get; private set; }

    public virtual ICollection<Order> OrdersCreated { get; private set; }
    public virtual ICollection<Order> OrdersClosed { get; private set; }
}
