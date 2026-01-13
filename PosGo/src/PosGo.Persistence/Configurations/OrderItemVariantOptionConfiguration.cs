using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class OrderItemVariantOptionConfiguration
    : IEntityTypeConfiguration<OrderItemVariantOption>
{
    public void Configure(EntityTypeBuilder<OrderItemVariantOption> builder)
    {
        builder.ToTable(TableNames.OrderItemVariantOptions);

        builder.HasKey(x => new { x.OrderItemId, x.VariantOptionId });

        // Snapshot properties (optional)
        builder.Property(x => x.VariantName)
               .HasMaxLength(200);

        builder.Property(x => x.OptionName)
               .HasMaxLength(200);

        builder.Property(x => x.PriceAdjustment)
               .HasColumnType("decimal(18,2)");

        // FK -> OrderItem
        builder.HasOne(x => x.OrderItem)
               .WithMany(oi => oi.VariantOptions)
               .HasForeignKey(x => x.OrderItemId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> DishVariantOption
        builder.HasOne(x => x.VariantOption)
               .WithMany() // VariantOption không cần navigation property ngược lại
               .HasForeignKey(x => x.VariantOptionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
