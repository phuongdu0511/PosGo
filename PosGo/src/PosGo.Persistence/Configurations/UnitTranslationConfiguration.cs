using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class UnitTranslationConfiguration
    : IEntityTypeConfiguration<UnitTranslation>
{
    public void Configure(EntityTypeBuilder<UnitTranslation> builder)
    {
        builder.ToTable(TableNames.UnitTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.HasIndex(x => new { x.UnitId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.Unit)
               .WithMany(u => u.Translations)
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.UnitTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
