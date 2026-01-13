using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishComboItemConfiguration : IEntityTypeConfiguration<DishComboItem>
{
    public void Configure(EntityTypeBuilder<DishComboItem> builder)
    {
        builder.ToTable(TableNames.DishComboItems);

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Quantity).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.IsRequired).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.DisplayOrder).IsRequired().HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => new { x.ComboDishId, x.DisplayOrder });
        builder.HasIndex(x => new { x.ComboDishId, x.ItemDishId }).IsUnique();

        // Foreign Keys
        builder.HasOne<Dish>()
               .WithMany() // Combo items
               .HasForeignKey(x => x.ComboDishId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Dish>()
               .WithMany() // Used in combos
               .HasForeignKey(x => x.ItemDishId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DishSku>()
               .WithMany()
               .HasForeignKey(x => x.ItemDishSkuId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
