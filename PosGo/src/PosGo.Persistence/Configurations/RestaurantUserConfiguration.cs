using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class RestaurantUserConfiguration
    : IEntityTypeConfiguration<RestaurantUser>
{
    public void Configure(EntityTypeBuilder<RestaurantUser> builder)
    {
        builder.ToTable(TableNames.RestaurantUsers);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.HasIndex(x => new { x.RestaurantId, x.UserId })
               .IsUnique();

        builder.HasIndex(x => x.UserId);

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(r => r.RestaurantUsers)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> User
        builder.HasOne(x => x.User)
               .WithMany(u => u.RestaurantUsers)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK -> Role
        builder.HasOne(x => x.Role)
               .WithMany(r => r.RestaurantUsers)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        // audit CreatedBy / UpdatedBy
        builder.HasOne(x => x.CreatedByUser)
               .WithMany(r => r.RestaurantUsersCreated)
               .HasForeignKey(x => x.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
               .WithMany(r => r.RestaurantUsersUpdated)
               .HasForeignKey(x => x.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
