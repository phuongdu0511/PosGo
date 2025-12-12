using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class UserSystemRoleConfiguration
    : IEntityTypeConfiguration<UserSystemRole>
{
    public void Configure(EntityTypeBuilder<UserSystemRole> builder)
    {
        builder.ToTable(TableNames.UserSystemRoles);

        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasOne(x => x.User)
               .WithMany(u => u.SystemRoles)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
               .WithMany(r => r.UserSystemRoles)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
