using AnimalShelter.Module.Animals.Domain;

namespace AnimalShelter.Module.Animals.Infrastructure.Database;

internal static class AnimalQueries
{
    public static IQueryable<Animal> Visible(this IQueryable<Animal> query)
        => query.Where(a => a.Status != AnimalStatus.NotAvailable);
}
