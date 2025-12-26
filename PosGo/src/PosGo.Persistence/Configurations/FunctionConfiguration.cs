using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Enums;
using PosGo.Persistence.Constants;
using PosGo.Domain.Entities;

namespace PosGo.Persistence.Configurations;

internal sealed class FunctionConfiguration : IEntityTypeConfiguration<Function>
{
    public void Configure(EntityTypeBuilder<Function> builder)
    {
        builder.ToTable(TableNames.Functions);

        builder.Property(x => x.Id).HasDefaultValue(0);

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired(true);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired(true);
        builder.Property(x => x.Key).HasMaxLength(50).IsRequired(true);
        builder.Property(x => x.ActionValue).IsRequired(true);
        builder.Property(x => x.ParrentId).HasDefaultValue(-1);
        builder.Property(x => x.CssClass).HasMaxLength(10).HasDefaultValue(null);
        builder.Property(x => x.Url).HasMaxLength(50).IsRequired(true);
        builder.Property(x => x.Status).HasDefaultValue(Status.Active);
        builder.Property(x => x.SortOrder).HasDefaultValue(-1);
    }
}
