using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantGroupConfiguration
    : IEntityTypeConfiguration<RestaurantGroup>
{
    public void Configure(EntityTypeBuilder<RestaurantGroup> builder)
    {
        builder.ToTable(TableNames.RestaurantGroups);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Slug)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
