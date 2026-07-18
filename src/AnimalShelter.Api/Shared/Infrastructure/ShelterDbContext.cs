using AnimalShelter.Api.Modules.Animals.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AnimalShelter.Api.Shared.Infrastructure;

public class ShelterDbContext : DbContext
{
    public DbSet<Animal> Animals => Set<Animal>();

    public ShelterDbContext(DbContextOptions<ShelterDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
