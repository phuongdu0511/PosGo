using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(TableNames.Languages);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(10)
               .IsRequired();

        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);
    }
}
