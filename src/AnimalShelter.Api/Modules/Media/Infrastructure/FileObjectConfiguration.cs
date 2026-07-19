using AnimalShelter.Api.Modules.Media.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalShelter.Api.Modules.Media.Infrastructure;

public class FileObjectConfiguration : IEntityTypeConfiguration<FileObject>
{
    public void Configure(EntityTypeBuilder<FileObject> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.StorageKey).HasMaxLength(500);
        builder.Property(f => f.OriginalFileName).HasMaxLength(255);
        builder.Property(f => f.ContentType).HasMaxLength(100);

        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.AccessLevel).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(f => f.StorageKey).IsUnique();
        builder.HasIndex(f => f.Status);
    }
}
