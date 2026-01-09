using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Persistence.Constants;
using PosGo.Domain.Entities;

namespace PosGo.Persistence.Configurations;

internal sealed class TableConfiguration
    : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable(TableNames.Tables);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => new { x.RestaurantId, x.Name })
               .IsUnique();

        builder.Property(x => x.QrCodeToken)
               .IsRequired();

        builder.HasIndex(x => x.QrCodeToken)
               .IsUnique();

        builder.Property(x => x.DoNotAllowOrder)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.MinOrderAmount)
               .HasColumnType("decimal(18,2)");

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.Tables)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Area (nullable)
        builder.HasOne(x => x.Area)
               .WithMany(a => a.Tables)
               .HasForeignKey(x => x.AreaId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
