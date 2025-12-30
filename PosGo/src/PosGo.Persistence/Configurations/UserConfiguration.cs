using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.UserName)
               .IsUnique();

        builder.Property(x => x.FullName)
               .HasMaxLength(200)
               .IsRequired();

        // Each User can have many UserClaims
        builder.HasMany(e => e.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(e => e.Logins)
            .WithOne()
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(e => e.Tokens)
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
    }
}
