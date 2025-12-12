using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishVariantTranslationConfiguration
    : IEntityTypeConfiguration<DishVariantTranslation>
{
    public void Configure(EntityTypeBuilder<DishVariantTranslation> builder)
    {
        builder.ToTable(TableNames.DishVariantTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.HasIndex(x => new { x.VariantId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.Variant)
               .WithMany(v => v.Translations)
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishVariantTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
