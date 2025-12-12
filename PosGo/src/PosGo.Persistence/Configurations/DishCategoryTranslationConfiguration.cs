using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishCategoryTranslationConfiguration
    : IEntityTypeConfiguration<DishCategoryTranslation>
{
    public void Configure(EntityTypeBuilder<DishCategoryTranslation> builder)
    {
        builder.ToTable(TableNames.DishCategoryTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.HasIndex(x => new { x.CategoryId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.Category)
               .WithMany(c => c.Translations)
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishCategoryTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
