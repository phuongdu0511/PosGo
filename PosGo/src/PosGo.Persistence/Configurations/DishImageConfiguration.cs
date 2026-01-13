using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishImageConfiguration : IEntityTypeConfiguration<DishImage>
{
    public void Configure(EntityTypeBuilder<DishImage> builder)
    {
        builder.ToTable(TableNames.DishImages);

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.IsPrimary).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.AltText).HasMaxLength(200);

        // Indexes
        builder.HasIndex(x => new { x.DishId, x.DisplayOrder });
        builder.HasIndex(x => new { x.DishId, x.IsPrimary });

        // Foreign Key
        builder.HasOne<Dish>()
               .WithMany() // Images collection
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
