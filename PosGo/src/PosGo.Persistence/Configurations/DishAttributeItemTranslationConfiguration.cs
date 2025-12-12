using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishAttributeItemTranslationConfiguration
    : IEntityTypeConfiguration<DishAttributeItemTranslation>
{
    public void Configure(EntityTypeBuilder<DishAttributeItemTranslation> builder)
    {
        builder.ToTable(TableNames.DishAttributeItemTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => new { x.AttributeItemId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.AttributeItem)
               .WithMany(i => i.Translations)
               .HasForeignKey(x => x.AttributeItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishAttributeItemTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
