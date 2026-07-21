using AnimalShelter.Api.Modules.Animals.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalShelter.Api.Modules.Animals.Infrastructure;

public class AnimalGalleryPhotoConfiguration : IEntityTypeConfiguration<AnimalGalleryPhoto>
{
    public void Configure(EntityTypeBuilder<AnimalGalleryPhoto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AnimalId);
        builder.Property(x => x.FileId);
        builder.HasOne<Animal>()
            .WithMany()
            .HasForeignKey(p => p.AnimalId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.AnimalId, p.FileId }).IsUnique();
    }
}
