using Microsoft.AspNetCore.Identity;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Role : IdentityRole<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public string Scope { get; set; } 
    public string Description { get; set; }
    public string RoleCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }

    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }
    public virtual ICollection<RestaurantUser> RestaurantUsers { get; private set; }
}
