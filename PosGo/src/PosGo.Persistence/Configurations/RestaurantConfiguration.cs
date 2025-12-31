using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantConfiguration
    : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable(TableNames.Restaurants);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Slug)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Address)
               .HasMaxLength(300);

        builder.Property(x => x.City)
               .HasMaxLength(100);

        builder.Property(x => x.Country)
               .HasMaxLength(100);

        builder.Property(x => x.Phone)
               .HasMaxLength(100);

        builder.Property(x => x.TimeZone)
               .HasMaxLength(50);

        builder.Property(x => x.LogoUrl)
               .HasMaxLength(500);

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        // FK -> RestaurantGroup (nullable)
        builder.HasOne(x => x.RestaurantGroup)
               .WithMany(g => g.Restaurants)
               .HasForeignKey(x => x.RestaurantGroupId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> DefaultLanguage
        builder.HasOne(x => x.DefaultLanguage)
               .WithMany(l => l.DefaultRestaurants)
               .HasForeignKey(x => x.DefaultLanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
