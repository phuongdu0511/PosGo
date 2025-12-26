using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableNames.Roles);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Scope)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(250).IsRequired(true);

        builder.Property(x => x.RoleCode).HasMaxLength(50).IsRequired(true);

        builder.HasIndex(x => x.RoleCode)
               .IsUnique();

        // Each User can have many RoleClaims
        builder.HasMany(e => e.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.RoleId)
            .IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
    }
}
