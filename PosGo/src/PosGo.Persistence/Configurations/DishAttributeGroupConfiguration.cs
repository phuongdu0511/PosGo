using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishAttributeGroupConfiguration
    : IEntityTypeConfiguration<DishAttributeGroup>
{
    public void Configure(EntityTypeBuilder<DishAttributeGroup> builder)
    {
        builder.ToTable(TableNames.DishAttributeGroups);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(50);

        builder.Property(x => x.IsRequired)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.IsMultipleSelection)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishAttributeGroups)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Dish
        builder.HasOne(x => x.Dish)
               .WithMany(d => d.AttributeGroups)
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
