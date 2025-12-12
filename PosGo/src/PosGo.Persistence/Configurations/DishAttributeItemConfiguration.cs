using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;
internal sealed class DishAttributeItemConfiguration
    : IEntityTypeConfiguration<DishAttributeItem>
{
    public void Configure(EntityTypeBuilder<DishAttributeItem> builder)
    {
        builder.ToTable(TableNames.DishAttributeItems);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(50);

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.IsDefault)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishAttributeItems)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> AttributeGroup
        builder.HasOne(x => x.AttributeGroup)
               .WithMany(g => g.Items)
               .HasForeignKey(x => x.AttributeGroupId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
