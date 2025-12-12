using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantLanguageConfiguration
    : IEntityTypeConfiguration<RestaurantLanguage>
{
    public void Configure(EntityTypeBuilder<RestaurantLanguage> builder)
    {
        builder.ToTable(TableNames.RestaurantLanguages);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsEnabled)
               .IsRequired()
               .HasDefaultValue(true);

        builder.HasIndex(x => new { x.RestaurantId, x.LanguageId })
               .IsUnique();

        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.RestaurantLanguages)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Language)
               .WithMany(l => l.RestaurantLanguages)
               .HasForeignKey(x => x.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
