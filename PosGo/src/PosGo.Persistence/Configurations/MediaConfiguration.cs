using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PosGo.Domain.Entities;
using PosGo.Persistence.Constants;

namespace PosGo.Persistence.Configurations;

internal sealed class MediaConfiguration
    : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable(TableNames.Media);

        builder.HasKey(x => x.Id);

        // ImageKey: S3 key - unique trong scope của restaurant
        builder.Property(x => x.ImageKey)
               .HasMaxLength(500)
               .IsRequired();

        // Unique index: ImageKey phải unique (vì S3 key là unique)
        builder.HasIndex(x => x.ImageKey)
               .IsUnique();

        // FileName: Original filename
        builder.Property(x => x.FileName)
               .HasMaxLength(255)
               .IsRequired();

        // ContentType: MIME type
        builder.Property(x => x.ContentType)
               .HasMaxLength(100)
               .IsRequired();

        // FileSize: Size in bytes
        builder.Property(x => x.FileSize)
               .IsRequired();

        // Folder: Context folder
        builder.Property(x => x.Folder)
               .HasMaxLength(50)
               .IsRequired();

        // FK -> Restaurant
        builder.HasOne(x => x.Restaurant)
               .WithMany(x => x.Media)
               .HasForeignKey(x => x.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
