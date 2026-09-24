using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Dtos;
using AnimalShelter.Module.Animals.Errors;
using AnimalShelter.Module.Animals.Infrastructure.Database;
using AnimalShelter.Module.Animals.Infrastructure.Providers;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Module.Animals.Features.GetAnimal;

internal sealed class GetAnimalHandler(
    AnimalsDbContext context,
    AnimalsMediaProvider mediaProvider)
    : IFeatureHandler<Guid, ErrorOr<AnimalDto>>
{
    public async Task<ErrorOr<AnimalDto>> HandleAsync(Guid animalId, CancellationToken ct)
    {
        var animal = await context.Animals.AsNoTracking()
            .Visible()
            .FirstOrDefaultAsync(a => a.Id == animalId, ct);
        if (animal is null)
            return AnimalErrors.NotFound(animalId);

        var profileImageUrl = animal.ProfileImageId is not null
            ? await mediaProvider.GetUrlAsync(animal.ProfileImageId.Value, ct)
            : null;

        return AnimalDto.From(animal, profileImageUrl?.ToString());
    }
}