using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(TableNames.Orders);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderCode)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => new { x.RestaurantId, x.OrderCode })
               .IsUnique();

        builder.Property(x => x.Note)
               .HasMaxLength(500);

        builder.Property(x => x.SubTotalAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.DiscountAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.FinalAmount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.Orders)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Table
        builder.HasOne(x => x.Table)
               .WithMany(t => t.Orders)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Status (CodeItem)
        builder.HasOne(x => x.Status)
               .WithMany(s => s.OrdersUsingStatus)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> CreatedBy / ClosedBy
        builder.HasOne(x => x.CreatedByUser)
               .WithMany(cbu => cbu.OrdersCreated)
               .HasForeignKey(x => x.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClosedByUser)
               .WithMany(cbu => cbu.OrdersClosed)
               .HasForeignKey(x => x.ClosedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.RestaurantId, x.StatusId, x.ClosedAt });
        builder.HasIndex(x => new { x.RestaurantId, x.CreatedAt });
    }
}
