using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishSkuConfiguration : IEntityTypeConfiguration<DishSku>
{
    public void Configure(EntityTypeBuilder<DishSku> builder)
    {
        builder.ToTable(TableNames.DishSkus);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.ImageUrl)
               .HasMaxLength(500);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.HasIndex(x => new { x.RestaurantId, x.IsActive, x.StockQuantity });

        // Indexes theo DBML: (DishId, Code) unique;
        builder.HasIndex(x => new { x.DishId, x.Sku })
               .IsUnique();

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishSkus)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Dish
        builder.HasOne(x => x.Dish)
               .WithMany(d => d.Skus)
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
