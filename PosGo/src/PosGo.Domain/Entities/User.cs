using System.Numerics;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Aggregates;
using PosGo.Domain.Exceptions;

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

    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersCreated { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersUpdated { get; private set; }

    public virtual ICollection<Order> OrdersCreated { get; private set; }
    public virtual ICollection<Order> OrdersClosed { get; private set; }

    public virtual ICollection<RolePermission> GrantedRolePermissions { get; private set; }
    public virtual ICollection<PermissionAssignment> AssignedPermissionAssignments { get; private set; }

    public User(Guid id, string userName, string password, string fullName, string phone, EUserStatus status)
    {
        Id = id;
        UserName = userName;
        Password = password;
        FullName = fullName;
        Phone = phone;
        Status = status;
    }

    public void UpdateProfile(string fullName, string phone)
    {
        FullName = fullName;
        Phone = phone;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new ArgumentNullException("Password hash không được rỗng.");
        }

        Password = newPasswordHash;

        // nếu muốn có domain event:
        // RaiseDomainEvent();
    }

    public static User CreateUser(Guid id, string userName, string password, string fullName, string phone) {
        var user = new User(id, userName, password, fullName, phone, EUserStatus.Active);
        //Raise Domain Event
        //...
        return user;
    }

    public void UpdateUser(string userName, string fullName, string phone)
    {
        UserName = userName;
        FullName = fullName;
        Phone = phone;
    }

    public void ChangeStatusUser(EUserStatus status)
    {
        Status = status;
    }
}
