using AnimalShelter.Module.Animals.Domain;
using ErrorOr;

namespace AnimalShelter.Module.Animals.Errors;

internal static class AnimalErrors
{
    public static Error NotFound(Guid animalId)
        => Error.NotFound($"Animal.NotFound", $"Animal {animalId} not found");

    public static Error UnavailableProfileMedia(Guid animalId, MediaFile file)
        => Error.Conflict("Animal.UnavailableProfileMedia", $"File {file.Id} of type {file.Kind.ToString()} cannot be profile media in animal {animalId}");
}
