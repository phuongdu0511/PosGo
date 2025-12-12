using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class OrderItemAttributeConfiguration
    : IEntityTypeConfiguration<OrderItemAttribute>
{
    public void Configure(EntityTypeBuilder<OrderItemAttribute> builder)
    {
        builder.ToTable(TableNames.OrderItemAttributes);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GroupName)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.ItemName)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => x.OrderItemId);

        builder.HasOne(x => x.OrderItem)
               .WithMany(o => o.Attributes)
               .HasForeignKey(x => x.OrderItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
