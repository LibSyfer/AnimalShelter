using AnimalShelter.Module.Animals.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Infrastructure.Database;

internal sealed class AnimalsDbContext : DbContext
{
    public const string Schema = "AnimalsModule";

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    

    public AnimalsDbContext(DbContextOptions<AnimalsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
    }
}
