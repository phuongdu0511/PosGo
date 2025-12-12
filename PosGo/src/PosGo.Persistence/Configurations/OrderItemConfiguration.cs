using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(TableNames.OrderItems);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DishName)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.DishUnit)
               .HasMaxLength(100);

        builder.Property(x => x.SkuCode)
               .HasMaxLength(50);

        builder.Property(x => x.SkuLabel)
               .HasMaxLength(200);

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(9,2)")
               .IsRequired();

        builder.Property(x => x.UnitPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.TotalPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Note)
               .HasMaxLength(500);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => new { x.RestaurantId, x.DishSkuId });

        // FK -> Order
        builder.HasOne(x => x.Order)
               .WithMany(o => o.Items)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Dish (snapshot, optional)
        builder.HasOne(x => x.Dish)
               .WithMany(d => d.OrderItems)
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> DishSku (optional)
        builder.HasOne(x => x.DishSku)
               .WithMany(ds => ds.OrderItems)
               .HasForeignKey(x => x.DishSkuId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
