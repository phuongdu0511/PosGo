using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable(TableNames.Dishes);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired(true);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.IsAvailable)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.ShowOnMenu)
               .IsRequired()
               .HasDefaultValue(true);

        builder.HasIndex(x => new { x.RestaurantId, x.CategoryId });

        builder.HasIndex(x => new { x.RestaurantId, x.IsActive, x.IsAvailable });  // Query menu
        builder.HasIndex(x => new { x.RestaurantId, x.CategoryId, x.SortOrder });  // Query by category
        builder.HasIndex(x => new { x.RestaurantId, x.DishTypeId });  // Query by type

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.Dishes)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Category (nullable)
        builder.HasOne(x => x.Category)
               .WithMany(c => c.Dishes)
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);

        // FK -> Unit (nullable)
        builder.HasOne(x => x.Unit)
               .WithMany(u => u.Dishes)
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.SetNull);

        // FK -> DishType (CodeItem, nullable)
        builder.HasOne(x => x.DishType)
               .WithMany(dt => dt.DishesUsingType)
               .HasForeignKey(x => x.DishTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
