using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class CodeSetConfiguration
    : IEntityTypeConfiguration<CodeSet>
{
    public void Configure(EntityTypeBuilder<CodeSet> builder)
    {
        builder.ToTable(TableNames.CodeSets);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.Name)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(500);
    }
}
