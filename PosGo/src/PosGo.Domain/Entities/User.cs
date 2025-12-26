using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public string? FullName { get;  set; }
    public EUserStatus Status { get;  set; } = EUserStatus.Active;
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersCreated { get; private set; }
    public virtual ICollection<RestaurantUser> RestaurantUsersUpdated { get; private set; }

    public virtual ICollection<Order> OrdersCreated { get; private set; }
    public virtual ICollection<Order> OrdersClosed { get; private set; }

    public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
    public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }

    //public User(Guid id, string userName, string password, string fullName, string phone, EUserStatus status)
    //{
    //    Id = id;
    //    UserName = userName;
    //    Password = password;
    //    FullName = fullName;
    //    Phone = phone;
    //    Status = status;
    //}

    public void UpdateProfile(string fullName, string phoneNumber)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    //public static User CreateUser(Guid id, string userName, string password, string fullName, string phone)
    //{
    //    var user = new User(id, userName, password, fullName, phone, EUserStatus.Active);
    //    return user;
    //}

    //public void UpdateUser(string userName, string fullName, string phone)
    //{
    //    UserName = userName;
    //    FullName = fullName;
    //    Phone = phone;
    //}

    //public void ChangeStatusUser(EUserStatus status)
    //{
    //    Status = status;
    //}
}
