using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class CodeItemConfiguration
    : IEntityTypeConfiguration<CodeItem>
{
    public void Configure(EntityTypeBuilder<CodeItem> builder)
    {
        builder.ToTable(TableNames.CodeItems);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => new { x.CodeSetId, x.Code })
               .IsUnique();

        builder.Property(x => x.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.SortOrder)
               .IsRequired()
               .HasDefaultValue(0);

        builder.HasOne(x => x.CodeSet)
               .WithMany(cs => cs.Items)
               .HasForeignKey(x => x.CodeSetId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
