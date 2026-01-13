using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishCategoryConfiguration
    : IEntityTypeConfiguration<DishCategory>
{
    public void Configure(EntityTypeBuilder<DishCategory> builder)
    {
        builder.ToTable(TableNames.DishCategories);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(50);

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.ShowOnMenu)
               .IsRequired()
               .HasDefaultValue(true);

        builder.HasIndex(x => new { x.RestaurantId, x.ParentCategoryId, x.SortOrder });  // Query tree
        builder.HasIndex(x => new { x.RestaurantId, x.IsActive });  // Query active categories

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.DishCategories)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // Self reference ParentCategory (nullable)
        builder.HasOne(x => x.ParentCategory)
               .WithMany(c => c.Children)
               .HasForeignKey(x => x.ParentCategoryId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
