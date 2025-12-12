using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.UserName)
               .IsUnique();

        builder.Property(x => x.Password)
               .HasMaxLength(300)
               .IsRequired();

        builder.Property(x => x.FullName)
               .HasMaxLength(200);

        builder.Property(x => x.Phone)
               .HasMaxLength(50);

        // self reference CreatedBy / UpdatedBy
        builder.HasOne(x => x.CreatedByUser)
               .WithMany(cbu => cbu.CreatedUsers)
               .HasForeignKey(x => x.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
               .WithMany(ubu => ubu.UpdatedUsers)
               .HasForeignKey(x => x.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
