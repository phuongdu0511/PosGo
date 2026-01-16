using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Persistence.Constants;
using PosGo.Domain.Entities;

namespace PosGo.Persistence.Configurations;

internal sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable(TableNames.Units);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => new { x.RestaurantId, x.Name })
               .IsUnique();

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // FK -> Restaurant (nullable)
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.Units)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
