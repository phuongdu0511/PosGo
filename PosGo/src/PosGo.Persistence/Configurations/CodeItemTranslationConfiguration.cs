using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class CodeItemTranslationConfiguration
    : IEntityTypeConfiguration<CodeItemTranslation>
{
    public void Configure(EntityTypeBuilder<CodeItemTranslation> builder)
    {
        builder.ToTable(TableNames.CodeItemTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => new { x.CodeItemId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.CodeItem)
               .WithMany(ci => ci.Translations)
               .HasForeignKey(x => x.CodeItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.CodeItemTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
