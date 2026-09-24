using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Domain;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using ErrorOr;

namespace AnimalShelter.Module.Animals.Features.CreateAnimal;

internal sealed record CreateAnimalRequest(
    string Name,
    string Species,
    string Gender,
    DateOnly? DateOfBirth,
    DateOnly? IntakeDate);

internal sealed class CreateAnimalHandler(
    AnimalsDbContext context,
    TimeProvider clock)
    : IFeatureHandler<CreateAnimalRequest, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateAnimalRequest request, CancellationToken ct)
    {
        var now = clock.GetUtcNow();

        var id = Guid.CreateVersion7();
        var species = Enum.Parse<AnimalSpecies>(request.Species, ignoreCase: true);
        var gender = Enum.Parse<AnimalGender>(request.Gender, ignoreCase: true);

        var animal = Animal.Create(
            id, request.Name,
            species, gender,
            request.DateOfBirth, request.IntakeDate,
            now);

        context.Animals.Add(animal);

        await context.SaveChangesAsync(ct);

        return animal.Id;
    }
}
