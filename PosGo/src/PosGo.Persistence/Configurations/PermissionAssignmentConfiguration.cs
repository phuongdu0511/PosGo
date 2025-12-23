using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class PermissionAssignmentConfiguration : IEntityTypeConfiguration<PermissionAssignment>
{
    public void Configure(EntityTypeBuilder<PermissionAssignment> builder)
    {
        builder.ToTable(TableNames.PermissionAssignments);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.FromRoleId, x.ToRoleId, x.PermissionId })
               .IsUnique();

        builder.Property(x => x.FromRoleId).IsRequired();
        builder.Property(x => x.ToRoleId).IsRequired();
        builder.Property(x => x.PermissionId).IsRequired();
        builder.Property(x => x.AssignedByUserId).IsRequired();

        builder.HasOne(x => x.FromRole)
               .WithMany(r => r.FromRolePermissionAssignments)
               .HasForeignKey(x => x.FromRoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToRole)
               .WithMany(r => r.ToRolePermissionAssignments)
               .HasForeignKey(x => x.ToRoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
               .WithMany(p => p.PermissionAssignments)
               .HasForeignKey(x => x.PermissionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedByUser)
               .WithMany(u => u.AssignedPermissionAssignments)
               .HasForeignKey(x => x.AssignedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
