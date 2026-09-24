namespace AnimalShelter.Module.Animals.Domain;

internal sealed record MediaFile(Guid Id, MediaFileKind Kind);

internal enum MediaFileKind
{
    Unsupported = 0,
    Image = 1,
    Video = 2
}
