using AnimalShelter.Api.Modules.Animals.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalShelter.Api.Modules.Animals.Infrastructure;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name)
            .HasMaxLength(50);
        builder.Property(a => a.Species)
            .HasMaxLength(30);
        builder.Property(a => a.DateOfBirth);
        builder.Property(a => a.IntakeDate);
        builder.Property(a => a.CreatedAt);
    }
}
