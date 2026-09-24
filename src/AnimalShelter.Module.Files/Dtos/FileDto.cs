using AnimalShelter.Common.Modules.Files;
using AnimalShelter.Module.Files.Domain;

namespace AnimalShelter.Module.Files.Dtos;

internal sealed record FileDto(
    Guid Id,
    string Status,
    string OriginalName,
    string ContentType,
    FileContentKind Kind,
    long Size,
    DateTimeOffset CreatedAt)
{
    public static FileDto From(FileObject file)
        => new(file.Id, file.Status.ToString(), file.OriginalName,
            file.ContentType, file.Kind,
            file.Size, file.CreatedAt);
}
