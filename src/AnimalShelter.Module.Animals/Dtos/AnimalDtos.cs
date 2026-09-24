using AnimalShelter.Module.Animals.Domain;

namespace AnimalShelter.Module.Animals.Dtos;

internal sealed record AnimalDto(
    Guid Id, string Species,
    string Gender, string Status,
    DateOnly? DateOfBirth, string? ProfileImageUrl,
    DateOnly IntakeDate)
{
    public static AnimalDto From(Animal animal, string? profileImageUrl)
        => new(animal.Id, animal.Species.ToString(),
            animal.Gender.ToString(), animal.Status.ToString(),
            animal.DateOfBirth, profileImageUrl, animal.IntakeDate);
}
