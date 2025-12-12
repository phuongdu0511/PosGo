using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishSkuVariantOptionConfiguration
    : IEntityTypeConfiguration<DishSkuVariantOption>
{
    public void Configure(EntityTypeBuilder<DishSkuVariantOption> builder)
    {
        builder.ToTable(TableNames.DishSkuVariantOptions);

        builder.HasKey(x => new { x.DishSkuId, x.VariantOptionId });

        builder.HasOne(x => x.DishSku)
               .WithMany(s => s.VariantOptions)
               .HasForeignKey(x => x.DishSkuId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VariantOption)
               .WithMany(vo => vo.DishSkuVariantOptions)
               .HasForeignKey(x => x.VariantOptionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
