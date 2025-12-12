using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishVariantOptionTranslationConfiguration
    : IEntityTypeConfiguration<DishVariantOptionTranslation>
{
    public void Configure(EntityTypeBuilder<DishVariantOptionTranslation> builder)
    {
        builder.ToTable(TableNames.DishVariantOptionTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => new { x.VariantOptionId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.VariantOption)
               .WithMany(o => o.Translations)
               .HasForeignKey(x => x.VariantOptionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishVariantOptionTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
