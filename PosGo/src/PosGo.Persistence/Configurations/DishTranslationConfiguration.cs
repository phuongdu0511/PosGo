using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class DishTranslationConfiguration
    : IEntityTypeConfiguration<DishTranslation>
{
    public void Configure(EntityTypeBuilder<DishTranslation> builder)
    {
        builder.ToTable(TableNames.DishTranslations);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(500);

        builder.HasIndex(x => new { x.DishId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.Dish)
               .WithMany(d => d.Translations)
               .HasForeignKey(x => x.DishId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.DishTranslations)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
