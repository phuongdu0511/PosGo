using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishVariantConfiguration
    : IEntityTypeConfiguration<DishVariant>
{
    public void Configure(EntityTypeBuilder<DishVariant> builder)
    {
        builder.ToTable(TableNames.DishVariants);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.HasIndex(x => new { x.DishId, x.Name })
               .IsUnique();

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishVariants)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Dish
        builder.HasOne(x => x.Dish)
               .WithMany(d => d.Variants)
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
