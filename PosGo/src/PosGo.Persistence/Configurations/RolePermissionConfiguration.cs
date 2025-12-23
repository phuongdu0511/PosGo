using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(TableNames.RolePermissions);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.RoleId, x.PermissionId })
               .IsUnique();

        builder.Property(x => x.RoleId)
               .IsRequired();

        builder.Property(x => x.PermissionId)
               .IsRequired();

        builder.Property(x => x.GrantedByUserId)
               .IsRequired();

        builder.HasOne(x => x.Role)
               .WithMany(r => r.RolePermissions)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(x => x.PermissionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.GrantedByUser)
               .WithMany(u => u.GrantedRolePermissions)
               .HasForeignKey(x => x.GrantedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
