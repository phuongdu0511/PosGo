using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishAttributeGroupTranslationConfiguration
    : IEntityTypeConfiguration<DishAttributeGroupTranslation>
{
    public void Configure(EntityTypeBuilder<DishAttributeGroupTranslation> builder)
    {
        builder.ToTable(TableNames.DishAttributeGroupTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => new { x.AttributeGroupId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.AttributeGroup)
               .WithMany(g => g.Translations)
               .HasForeignKey(x => x.AttributeGroupId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishAttributeGroupTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
