using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable(TableNames.UserRoles);

        builder.HasKey(x => new { x.RoleId, x.UserId });
    }
}

internal sealed class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable(TableNames.RoleClaims);

        //builder.HasKey(x => x.RoleId);
        builder.HasKey(x => x.Id);
    }
}

internal sealed class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable(TableNames.UserClaims);

        //builder.HasKey(x => x.UserId);
        builder.HasKey(x => x.Id);
    }
}

internal sealed class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable(TableNames.UserLogins);

        builder.HasKey(x => x.UserId);
    }
}

internal sealed class UserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable(TableNames.UserTokens);

        builder.HasKey(x => x.UserId);
    }
}
