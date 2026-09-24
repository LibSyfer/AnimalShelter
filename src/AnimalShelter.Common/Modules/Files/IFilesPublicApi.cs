namespace AnimalShelter.Common.Modules.Files;

public interface IFilesPublicApi
{
    Task<FileMetadata?> GetMetadataAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, FileMetadata>> GetManyMetadataAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<Uri?> GetContentUrlAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, Uri>> GetManyContentUrlsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
}

public sealed record FileMetadata(
    Guid Id,
    string Status,
    string Name,
    string ContentType,
    FileContentKind Kind);

public enum FileContentKind
{
    Unknown = 0,
    Image = 1,
    Video = 2,
    Audio = 3,
    Document = 4
}
