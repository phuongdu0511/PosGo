using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantPlanConfiguration : IEntityTypeConfiguration<RestaurantPlan>
{
    public void Configure(EntityTypeBuilder<RestaurantPlan> builder)
    {
        builder.ToTable(TableNames.RestaurantPlans);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => new { x.RestaurantId, x.PlanId });

        builder.HasOne(x => x.Restaurant)
            .WithMany(p => p.RestaurantPlans)
            .HasForeignKey(x => x.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Plan)
            .WithMany(x => x.RestaurantPlans)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
