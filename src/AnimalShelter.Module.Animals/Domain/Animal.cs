using AnimalShelter.Common.Abstracts;
using AnimalShelter.Module.Animals.Errors;
using ErrorOr;

namespace AnimalShelter.Module.Animals.Domain;

internal sealed class Animal : EntityBase
{
    public string Name { get; private set; } = null!;
    public AnimalSpecies Species { get; private set; }
    public AnimalGender Gender { get; private set; }
    public AnimalStatus Status { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public Guid? ProfileImageId { get; private  set; }

    public DateOnly IntakeDate { get; private set; }

    private Animal() { }

    public static Animal Create(
        Guid id, string Name,
        AnimalSpecies species, AnimalGender gender,
        DateOnly? dateOfBirth, DateOnly? intakeDate,
        DateTimeOffset now)
    {
        return new Animal
        {
            Id = id,
            Name = Name,
            Species = species,
            Gender = gender,
            Status = AnimalStatus.Available,
            DateOfBirth = dateOfBirth,
            IntakeDate = DateOnly.FromDateTime(now.DateTime),
            CreatedAt = now
        };
    }

    public ErrorOr<Success> SetProfileImage(MediaFile? mediaFile)
    {
        if (mediaFile is { } && mediaFile.Kind != MediaFileKind.Image)
            return AnimalErrors.UnavailableProfileMedia(Id, mediaFile);

        ProfileImageId = mediaFile?.Id;

        return Result.Success;
    }

    public void RemoveProfileImage()
        => ProfileImageId = null;
}