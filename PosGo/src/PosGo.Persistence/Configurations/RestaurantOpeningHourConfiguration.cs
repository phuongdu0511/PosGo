using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantOpeningHourConfiguration
    : IEntityTypeConfiguration<RestaurantOpeningHour>
{
    public void Configure(EntityTypeBuilder<RestaurantOpeningHour> builder)
    {
        builder.ToTable(TableNames.RestaurantOpeningHours);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
               .IsRequired();

        builder.Property(x => x.OpenTime)
               .HasColumnType("time");

        builder.Property(x => x.CloseTime)
               .HasColumnType("time");

        builder.Property(x => x.IsClosed)
               .IsRequired()
               .HasDefaultValue(false);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.OpeningHours)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.RestaurantId, x.DayOfWeek })
               .IsUnique();
    }
}
