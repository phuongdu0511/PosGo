using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishVariantOptionConfiguration
    : IEntityTypeConfiguration<DishVariantOption>
{
    public void Configure(EntityTypeBuilder<DishVariantOption> builder)
    {
        builder.ToTable(TableNames.DishVariantOptions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => new { x.VariantId, x.Value })
               .IsUnique();

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.IsDefault)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.PriceAdjustment)
               .HasColumnType("decimal(18,2)")
               .IsRequired()
               .HasDefaultValue(0);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishVariantOptions)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Variant
        builder.HasOne(x => x.Variant)
               .WithMany(v => v.Options)
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
