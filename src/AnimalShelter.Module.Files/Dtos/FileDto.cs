using AnimalShelter.Module.Files.Domain;

namespace AnimalShelter.Module.Files.Dtos;

internal sealed record FileDto(
    Guid Id,
    FileObjectStatus Status,
    string OriginalName,
    string ContentType,
    FileObjectKind Kind,
    long Size,
    DateTimeOffset CreatedAt)
{
    public static FileDto From(FileObject file)
        => new(file.Id, file.Status, file.OriginalName,
            file.ContentType, file.Kind,
            file.Size, file.CreatedAt);
}
