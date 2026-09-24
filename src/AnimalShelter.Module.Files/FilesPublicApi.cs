using AnimalShelter.Common.PublicApis;
using AnimalShelter.Module.Files.Domain;
using AnimalShelter.Module.Files.Dtos;
using AnimalShelter.Module.Files.Features.GetContentUrl;
using AnimalShelter.Module.Files.Features.GetFile;
using AnimalShelter.Module.Files.Features.GetManyContentUrlsHandler;
using AnimalShelter.Module.Files.Features.GetManyFiles;

namespace AnimalShelter.Module.Files;

internal sealed class FilesPublicApi(
    GetReadyFileHanlder getFile,
    GetManyReadyFilesHandler getManyFiles,
    GetContentUrlHandler getContentUrl,
    GetManyContentUrlsHandler getManyContentUrls)
    : IFilesPublicApi
{
    public async Task<Uri?> GetContentUrlAsync(Guid id, CancellationToken ct)
    {
        var result = await getContentUrl.HandleAsync(id, ct);
        return result.IsSuccess ? result.Value : null;
    }

    public Task<IReadOnlyDictionary<Guid, Uri>> GetManyContentUrlsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
        => getManyContentUrls.HandleAsync(ids, ct);

    public async Task<IReadOnlyDictionary<Guid, FileMetadata>> GetManyMetadataAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct)
    {
        var result = await getManyFiles.HandleAsync(ids, ct);
        return result.ToDictionary(kv => kv.Key, kv => ToFileMetadata(kv.Value));
    }

    public async Task<FileMetadata?> GetMetadataAsync(Guid id, CancellationToken ct)
    {
        var result = await getFile.HandleAsync(id, ct);
        return result.IsSuccess ? ToFileMetadata(result.Value) : null;
    }

    private static FileMetadata ToFileMetadata(FileDto dto)
        => new(dto.Id, dto.OriginalName, Map(dto.Kind));

    private static FileKind Map(FileObjectKind kind) => kind switch
    {
        FileObjectKind.Image => FileKind.Image,
        FileObjectKind.Video => FileKind.Video,
        FileObjectKind.Unknown => FileKind.Unknown,
        _ => FileKind.Unknown
    };
}
