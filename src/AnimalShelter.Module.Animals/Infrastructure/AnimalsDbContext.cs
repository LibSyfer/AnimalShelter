using AnimalShelter.Module.Animals.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Infrastructure;

internal sealed class AnimalsDbContext : DbContext
{
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalMedia> Media => Set<AnimalMedia>();
    

    public AnimalsDbContext(DbContextOptions<AnimalsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
