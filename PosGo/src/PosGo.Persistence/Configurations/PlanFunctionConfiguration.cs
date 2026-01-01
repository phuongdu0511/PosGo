using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class PlanFunctionConfiguration : IEntityTypeConfiguration<PlanFunction>
{
    public void Configure(EntityTypeBuilder<PlanFunction> builder)
    {
        builder.ToTable(TableNames.PlanFunctions);

        builder.HasKey(x => x.Id);

        // Tạm thời để là 15 full quyền
        builder.Property(x => x.ActionValue)
            .HasDefaultValue(15);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // 1 plan không được trùng function
        builder.HasIndex(x => new { x.PlanId, x.FunctionId })
            .IsUnique();

        builder.HasOne(x => x.Plan)
            .WithMany(x => x.PlanFunctions)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Function)
            .WithMany(f => f.PlanFunctions)
            .HasForeignKey(x => x.FunctionId)
            .OnDelete(DeleteBehavior.Restrict); // tránh xóa Function kéo theo plan
    }
}
